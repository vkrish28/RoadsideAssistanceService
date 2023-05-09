using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ERAEntities.DataModels
{
    public class Assistant
    {
        [Key]
        public int Id { get; set; }
        public string ServiceProviderName { get; set; }
        public string? Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public int Zipcode { get; set; }
    }
}
