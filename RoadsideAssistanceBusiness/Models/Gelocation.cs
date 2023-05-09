using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Models
{
    /// <summary>
    /// Basic gelocation that holds longitude and lattitude
    /// </summary>
    public class Geolocation
    {
        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Lattitude
        /// </summary>
        public double Lattitude { get; set; }
    }
}
