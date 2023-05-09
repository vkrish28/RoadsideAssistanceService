namespace EmergencyAssistanceAPI.Models
{
    /// <summary>
    /// The request model to be used with Update Assitant Location API
    /// </summary>
    public class UpdateAssitantLocationRequestModel
    {
        /// <summary>
        /// The Id of the service provider
        /// </summary>
        /// <example>1</example>
        public int AssistantId { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        /// <example>-92.180929</example>
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        /// <example>30.23998</example>
        public double Latitude { get; set; }

    }
}
