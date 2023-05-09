using EmergencyAssistanceAPI.Models;
using ERAEntities.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoadsideAssistanceBusiness.Interfaces;
using RoadsideAssistanceBusiness.Models;
using Assistant = RoadsideAssistanceBusiness.Models.Assistant;
using Customer = RoadsideAssistanceBusiness.Models.Customer;

namespace RoadsideAssistanceService.Controllers
{
    /// <summary>
    /// API Controller for the Emergency Roadside Assitance service
    /// </summary>
    [ApiController]
    public class RoadsideAssistanceController : ControllerBase
    {
        // incase we need any config values in here
        private readonly IConfiguration _configuration;        
        private readonly IRoadsideAssistanceService _roadsideAssistanceService;

        /// <summary>
        /// Roadside Assistance Controller
        /// </summary>
        /// <param name="config">App config object</param>
        /// <param name="roadsideAssistanceService">Roadside Assistance Service object</param>
        public RoadsideAssistanceController(IConfiguration config, IRoadsideAssistanceService roadsideAssistanceService)
        {
            _configuration = config;
            _roadsideAssistanceService = roadsideAssistanceService;
        }

        /// <summary>
        /// Retrieves the nearest available service providers for the given coordinates
        /// </summary>
        /// <param name="nearestAssistantsModel"> The request model for finding the nearest assitants/service providers (Longitude, Lattitude and max number of results to return)</param>
        /// <returns>List of specified # of serivce providers nearby</returns>
        [HttpPost, Route("NearestAssitants"), Produces("application/json")]
        [ProducesResponseType(typeof(NearestAssistantsResponseModel), StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> NearestAssitants([FromBody] NearestAssistantsRequestModel nearestAssistantsModel)
        {
            try
            {
                //validate input coordinates
                if(nearestAssistantsModel.CustomerLongitude == 0 || nearestAssistantsModel.CustomerLatitude == 0)
                {
                    return BadRequest("Invalid customer location");
                }
                var geoLocation = new Geolocation{ Longitude = nearestAssistantsModel.CustomerLongitude, Lattitude = nearestAssistantsModel.CustomerLatitude };
                
                //validate # of providers to return
                var limit = 5; // default to 5
                if(nearestAssistantsModel.Limit > 0)
                {
                    limit = nearestAssistantsModel.Limit;
                }

                // call business service
                var results = await _roadsideAssistanceService.FindNearestAssistants(geoLocation, limit);

                // process the response
                var response = new NearestAssistantsResponseModel { NearestAssistants = results, StatusCode = 200, Success = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // log the exception

                var result = new ObjectResult(new { message = ex.Message });
                result.StatusCode = 400; 
                return result;
            }
        }

        /// <summary>
        /// Updates location of an Assistant
        /// </summary>
        /// <param name="updateAssitantLocationRequestModel"> The request model for holding Assistant's Id, current location longitude and lattitude</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// POST /UpdateAssistantLocation
        /// {
        ///     "AssistantId": 1,
        ///     "Longitude": -92.180929,
        ///     "Latitude": 30.23998
        ///  }   
        /// </remarks>
        [HttpPost, Route("UpdateAssistantLocation"), Produces("application/json")]
        [ProducesResponseType(200), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAssistantLocation([FromBody] UpdateAssitantLocationRequestModel updateAssitantLocationRequestModel)
        {
            try
            {
                // input validation
                if (updateAssitantLocationRequestModel == null || updateAssitantLocationRequestModel.AssistantId <= 0 ||
                    updateAssitantLocationRequestModel.Longitude == 0 || updateAssitantLocationRequestModel.Latitude == 0)
                {
                    return BadRequest("Assistant or location details");
                }

                // prep the models
                var assistant = new Assistant() { Id = updateAssitantLocationRequestModel.AssistantId };
                var geolocation = new Geolocation() { Lattitude = updateAssitantLocationRequestModel.Latitude, Longitude = updateAssitantLocationRequestModel.Longitude };

                // call the business service
                await _roadsideAssistanceService.UpdateAssistantLocation(assistant, geolocation);

                // process the response
                 return Ok(new {Statuscode = 200, Success = true});
            }
            catch (Exception ex)
            {
                // log the exception

                var result = new ObjectResult(new { message = ex.Message });
                result.StatusCode = 400; 
                return result;
            }
        }

        /// <summary>
        /// Reserves nearest assistant for a customer based on the customer location
        /// </summary>
        /// <param name="reserveAssistantRequestModel">The reserve model which at the minimum should have CustomerId, customer longitude & lattitude</param>
        /// <returns></returns>
        [HttpPost, Route("ReserveAssistant"), Produces("application/json")]
        [ProducesResponseType(typeof(ReserveAssistantResponseModel), StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ReserveAssistant([FromBody] ReserveAssistantRequestModel reserveAssistantRequestModel)
        {
            try
            {
                //validate input coordinates
                if (reserveAssistantRequestModel.CustomerId <= 0 || reserveAssistantRequestModel.CustomerLongitude == 0 
                    || reserveAssistantRequestModel.CustomerLatitude == 0)
                {
                    return BadRequest("Invalid customer details");
                }

                // prep the models
                var customer = new Customer { Id = reserveAssistantRequestModel.CustomerId };
                var custLocation = new Geolocation { Lattitude = reserveAssistantRequestModel.CustomerLatitude,
                                                     Longitude = reserveAssistantRequestModel.CustomerLongitude };
                // call the business service
                var result = await _roadsideAssistanceService.ReserveAssistant(customer, custLocation);

                // process the response
                var response = new ReserveAssistantResponseModel { ReservedAssistant = result, StatusCode = 200, Success = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // log the exception

                var result = new ObjectResult(new { message = ex.Message });
                result.StatusCode = 400;
                return result;
            }
        }

        /// <summary>
        /// Releases an assitant from a customer
        /// </summary>
        /// <param name="releaseAssitantLocationRequestModel">The request body model, customerId and AssistantId are required </param>
        /// <returns></returns>
        [HttpPost, Route("ReleaseAssistant"), Produces("application/json")]
        [ProducesResponseType(200), ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ReleaseAssistant([FromBody] ReleaseAssitantLocationRequestModel releaseAssitantLocationRequestModel)
        {
            try
            {
                // input validation
                if (releaseAssitantLocationRequestModel == null || releaseAssitantLocationRequestModel.AssistantId <= 0 ||
                    releaseAssitantLocationRequestModel.CustomerId <= 0 )
                {
                    return BadRequest("Invalid Assistant or Customer");
                }

                // prep the models
                var assistant = new Assistant() { Id = releaseAssitantLocationRequestModel.AssistantId };
                var customer = new Customer() { Id = releaseAssitantLocationRequestModel.CustomerId };

                // call the business service
                await _roadsideAssistanceService.ReleaseAssistant(customer, assistant);

                // process the response
                return Ok(new { Statuscode = 200, Success = true });
            }
            catch (Exception ex)
            {
                // log the exception

                var result = new ObjectResult(new { message = ex.Message });
                result.StatusCode = 400;
                return result;
            }
        }
    }
}
