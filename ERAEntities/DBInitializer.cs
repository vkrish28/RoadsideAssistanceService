using ERAEntities.DataModels;
using ERAEntities.RawData;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERAEntities
{
    public static class DBInitializer
    {
        public static void Init(ERAContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Assistants.Any())
            {
                loadAssitants(context);
            }

            if(!context.Customers.Any())
            {
                loadCustomers(context);
            }
        }

        private static void loadCustomers(ERAContext context) 
        {
            //sample customers
            var customers = new List<Customer> {
                new Customer { Name="James", Phone="111-111-1111"},
                new Customer { Name="Adam",  Phone="111-111-1212"},
                new Customer { Name="Mike", Phone="111-111-1213"},
                new Customer { Name="Steve", Phone="111-111-1214"},
                new Customer { Name="John", Phone="111-111-1215"},
                new Customer { Name="Paul", Phone="111-111-1216"},
                new Customer { Name="Kim", Phone="111-111-1217"},
                new Customer { Name="Alissa", Phone="111-111-1218"},
                new Customer { Name="Dita", Phone="111-111-1219"},
            };
            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
        private static void loadAssitants(ERAContext context)
        {
            // sample assistants from json file
            var jsonData = System.IO.File.ReadAllText("ServiceProviders.json");

            var providers = JsonConvert.DeserializeObject<List<Providers>>(jsonData);

            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            foreach (var provider in providers)
            {
                Assistant asst = new Assistant
                {
                    ServiceProviderName = provider.ESP_NAME,
                    Address = provider.ADDRESS,
                    City = provider.CITY,
                    Zipcode = provider.ZIPCODE,
                    County = provider.COUNTY,
                    State = provider.STATE,
                    Phone = provider.PHONE
                };

                context.Assistants.Add(asst);
                context.SaveChanges();
                AssistantLocation astLoc = new AssistantLocation
                {
                    AssistantId = asst.Id,
                    Geolocation = geoFactory.CreatePoint(new Coordinate(provider.LONGITUDE.Value, provider.LATITUDE.Value)),
                    Status = (int)AssistantStatus.Released
                };
                context.AssistantGeoLocations.Add(astLoc);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    continue;
                }

            }

        }
    }
}
