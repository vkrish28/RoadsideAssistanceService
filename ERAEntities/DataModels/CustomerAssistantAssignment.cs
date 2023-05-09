using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERAEntities.DataModels
{
    public class CustomerAssistantAssignment
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AssitantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
