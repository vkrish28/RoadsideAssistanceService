using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERAEntities.RawData
{
    internal class Providers
    {
        public int ID { get; set; }
        public string ESP_NAME { get; set; }

        public string PHONE { get; set; }

        public string ADDRESS { get; set; }

        public string CITY { get; set; }

        public string COUNTY { get; set; }

        public string STATE { get; set; }

        public int ZIPCODE { get; set; }

        public double? LATITUDE { get; set; }

        public double? LONGITUDE { get; set; }

        public int? ESP_SCORE { get; set; }
    }
}
