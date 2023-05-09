using ERAEntities.DataModels;

namespace EmergencyAssistanceAPI.Models
{
    /// <summary>
    /// The request model for the Release Assistant API
    /// </summary>
    public class ReleaseAssitantLocationRequestModel
    {
        /// <summary>
        /// Id of the Customer
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// Id of the Assistant
        /// </summary>
        public int AssistantId { get; set; }
    }
}
