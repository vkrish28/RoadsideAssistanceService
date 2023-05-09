using RoadsideAssistanceBusiness.Models;

namespace EmergencyAssistanceAPI.Models
{
    /// <summary>
    /// The response model of the Nearest Assitants API
    /// </summary>
    public class NearestAssistantsResponseModel
    {
        /// <summary>
        /// Success or Failure
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// http status code
        /// </summary>
        public int StatusCode{ get; set; }

        /// <summary>
        /// List of Nearest Assistants
        /// </summary>
        public List<Assistant> NearestAssistants { get; set; }
      
    }
}