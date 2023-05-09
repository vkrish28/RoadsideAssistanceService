using RoadsideAssistanceBusiness.Models;

namespace EmergencyAssistanceAPI.Models
{
    public class ReserveAssistantResponseModel
    {
        /// <summary>
        /// Success or Failure
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// http status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The reserved assistant
        /// </summary>
        public Assistant? ReservedAssistant { get; set; }
    }
}
