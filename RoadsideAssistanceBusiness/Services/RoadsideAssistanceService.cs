using RoadsideAssistanceBusiness.Interfaces;
using RoadsideAssistanceBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using ERAEntities;
using NetTopologySuite;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using ERAEntities.DataModels;
using Assistant = RoadsideAssistanceBusiness.Models.Assistant;
using Customer = RoadsideAssistanceBusiness.Models.Customer;
using Microsoft.Extensions.Configuration;

namespace RoadsideAssistanceBusiness.Services
{
    /// <summary>
    /// The Emergency Roadside Assitance Service implementtion
    /// </summary>
    public class RoadsideAssistanceService : IRoadsideAssistanceService
    {
        private readonly ERAContext _context;
        private readonly GeometryFactory _geoFactory;
        private readonly IConfiguration _configuration;
        private readonly IEntityFactory _entityFactory;

        public RoadsideAssistanceService(IConfiguration config, IEntityFactory entityFactory)
        {
            _configuration = config;
            _entityFactory = entityFactory;
            _geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            _context = GetDbContext();
        }

        /// <inheritdoc cref="IRoadsideAssistanceService.FindNearestAssistants(Geolocation, int)" />
        public async Task<List<Assistant>> FindNearestAssistants(Geolocation geolocation, int limit)
        {
            var result = new List<Assistant>();

            // input validation
            if(geolocation == null ||  geolocation.Lattitude < -90 || geolocation.Lattitude > 90 ||
                geolocation.Longitude < -180 || geolocation.Longitude > 180)
            {
                throw new ArgumentNullException("Invalid location data");
            }


            // create Geometry point based on the coordinates of the disabled vehicle
            var disabledLocation = _geoFactory.CreatePoint(new Coordinate(geolocation.Longitude, geolocation.Lattitude));

            var dbContext = GetDbContext();

            // find the nearest assitants based on the geometry point
            result = await _context.AssistantGeoLocations.Where(a=>a.Status != (int)AssistantStatus.Reserved) // Reserved for a customer
                .Select(agl => new
                        {
                           agl.AssistantId,
                           agl.Geolocation,
                           Distance = agl.Geolocation.Distance(disabledLocation) // Calculate the distance based on geo points ( 'Geolocation" datatype in SQL server )

                        })
                .OrderBy(x => x.Distance).Take(limit)  // limit
                .Join(_context.Assistants, agl => agl.AssistantId, a => a.Id, (agl, a) => new Assistant
                        {
                            //explore the provider details
                            Id = agl.AssistantId,
                            ServiceProviderName = a.ServiceProviderName,
                            Distance = Math.Round(agl.Distance / 1609.34, 2),
                            Longitude = agl.Geolocation.Coordinate.X,
                            Latitude = agl.Geolocation.Coordinate.Y,
                            Phone = a.Phone,
                            Address = a.Address,
                            City = a.City,
                            Zipcode = a.Zipcode,
                            County = a.County
                        }).ToListAsync();
            return result;
            
        }

        /// <inheritdoc cref="IRoadsideAssistanceService.ReleaseAssistant(Customer, Assistant)"/>
        public async Task ReleaseAssistant(Customer customer, Assistant assistant)
        {
            // input validation
            if (customer == null || customer.Id <= 0 )
            {
                throw new ArgumentNullException("Customer details");
            }
            if (assistant == null || assistant.Id <= 0)
            {
                throw new ArgumentNullException("Assistant details");
            }

            // make sure customer and assistant exists in the system
            var cust = await _context.Customers.Where(x => x.Id == customer.Id).FirstOrDefaultAsync();
            var ast = await _context.Assistants.Where(x => x.Id == assistant.Id).FirstOrDefaultAsync();

            if(cust == null || ast == null)
            {
                throw new ArgumentException(" Customer/Assistnat is not found");
            }

            // now pull the active reservation, ideally this should be only one
            var reservations = await _context.CustomerAssistantAssignments
                .Where(x => x.AssitantId == ast.Id && x.CustomerId == customer.Id && x.EndTime == null).ToListAsync();

            if(reservations.Count() == 0)
            {
                throw new ArgumentException("No active reservation");
            }

            foreach (var reservation in reservations)
            {
                reservation.EndTime = DateTime.UtcNow;
                var assistantLoc = await _context.AssistantGeoLocations.Where(x => x.AssistantId == reservation.AssitantId).FirstAsync();
                assistantLoc.Status = (int)AssistantStatus.Released;
                _context.AssistantGeoLocations.Update(assistantLoc);
            }

            _context.CustomerAssistantAssignments.UpdateRange(reservations);
            await _context.SaveChangesAsync();

        }

        /// <inheritdoc cref="IRoadsideAssistanceService.ReserveAssistant(Customer, Geolocation)"/>
        public async Task<Assistant?> ReserveAssistant(Customer customer, Geolocation customerLocation)
        {
            // input validation
            if (customer == null || customerLocation == null ||
                customerLocation.Lattitude < -90 || customerLocation.Lattitude > 90 ||
                customerLocation.Longitude < -180 || customerLocation.Longitude > 180)
            {
                throw new ArgumentNullException("Customer or location details");
            }

            // make sure customer exists
            var cust = await _context.Customers.Where(c => c.Id == customer.Id).FirstAsync();
            if (cust == null)
            {
                throw new Exception("Customer not found");
            }

            // make sure an assistant is not already reserved for this cusotmer
            var existingReservation = await _context.CustomerAssistantAssignments
                .Where(x => x.CustomerId == customer.Id && x.EndTime == null).FirstOrDefaultAsync(); // An entry with no endtime indicates that  reservation is active
            if(existingReservation != null)
            {
                throw new Exception("The customer have an active resrvation");
            }

            // find the nearest assistant
            var nearestAssistant = await FindNearestAssistants(customerLocation, 1);

            if (nearestAssistant == null || nearestAssistant.Count == 0)
            {
                throw new Exception("No Assistants available near the given location");
            }

            // Change the status of the Assistant to Reserved 
             var assistantGelocation = await _context.AssistantGeoLocations.Where(x => x.AssistantId == nearestAssistant.First().Id 
                                                                    && x.Status == (int)AssistantStatus.Released).FirstOrDefaultAsync();

            if(assistantGelocation == null)
            {
                throw new Exception("Failed to reserve the assistant");
            }

            assistantGelocation.Status = (int)AssistantStatus.Reserved;
            _context.AssistantGeoLocations.Update(assistantGelocation);

            // create reservation entry
            var reservation = new CustomerAssistantAssignment
            {
                AssitantId = assistantGelocation.AssistantId,
                CustomerId = cust.Id,
                StartTime = DateTime.UtcNow
            };
            _context.CustomerAssistantAssignments.Add(reservation);
            await _context.SaveChangesAsync();

            return nearestAssistant.First();
        }

        /// <inheritdoc cref="IRoadsideAssistanceService.UpdateAssistantLocation(Assistant, Geolocation) " />
        public async Task UpdateAssistantLocation(Assistant assistant, Geolocation assistantLocation)
        {
            // input validation
            if(assistant == null || assistantLocation == null)
            {
                throw new ArgumentNullException("Assistant or location details");
            }
            if (assistantLocation.Lattitude < -90 || assistantLocation.Lattitude > 90 ||
                assistantLocation.Longitude < -180 || assistantLocation.Longitude > 180)
            {
                throw new ArgumentException("Invalid longitude or latitude");
            }

            // create Geometry point from input location
            var assistantCurrentGeoLocation = _geoFactory.CreatePoint(new Coordinate(assistantLocation.Longitude, assistantLocation.Lattitude));

            //retrieve the Assistant's current location from db 
            var assistantGelocation = _context.AssistantGeoLocations.Where( a => a.Id == assistant.Id).FirstOrDefault();

            if(assistantGelocation == null)
            {
                throw new ArgumentException("Assitant is not found");
            }
            // update location
            assistantGelocation.Geolocation = assistantCurrentGeoLocation;
            _context.AssistantGeoLocations.Update(assistantGelocation);
            await _context.SaveChangesAsync();
            
        }

        private ERAContext GetDbContext()
        {
            var connString = _configuration.GetConnectionString("EmergencyRoadsideAssistanceModel");
            return _entityFactory.CreateDbConext(connString);

        }
    }
}
