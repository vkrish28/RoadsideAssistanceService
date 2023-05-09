namespace EmergencyAssistanceAPI.Models
{
    /// <summary>
    /// the request model for the ReserveAssistant API
    /// </summary>
    public class ReserveAssistantRequestModel
    {
        /// <summary>
        /// Id of the customer
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Longitude of Customer's abandoned vehicle location 
        /// </summary>
        public double CustomerLongitude { get; set; }

        /// <summary>
        /// Latitude of Customer's abandoned vehicle location 
        /// </summary>
        public double CustomerLatitude { get; set; }

    }
}
