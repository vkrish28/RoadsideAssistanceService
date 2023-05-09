using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace ERAEntities.DataModels
{
    public class AssistantLocation
    {
        [Key]
        public int Id { get; set; }
        public int AssistantId { get; set; }
        public Point Geolocation { get; set; }
        public int Status { get; set; }
    }

    public enum AssistantStatus
    {
        Released = 0,
        Reserved = 1
    }
}
