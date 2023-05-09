namespace EmergencyAssistanceAPI.Models
{
    /// <summary>
    /// The request model to be used with Nearest Assitants API
    /// </summary>
    public class NearestAssistantsRequestModel
    {

        /// <summary>
        /// Longitude of Customer's abandoned vehicle location 
        /// </summary>
        public double CustomerLongitude { get; set; }

        /// <summary>
        /// Latitude of Customer's abandoned vehicle location 
        /// </summary>
        public double CustomerLatitude { get; set; }

        /// <summary>
        /// Max number of results(nearest Assitants) to return with the search request
        /// </summary>
        public int Limit { get; set; }
    }
}
