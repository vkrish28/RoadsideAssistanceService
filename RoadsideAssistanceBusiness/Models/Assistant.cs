using ERAEntities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Models
{
    /// <summary>
    /// Business Model for Service Provider
    /// </summary>
    public class Assistant
    {
        /// <summary>
        /// Identification
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///Business name
        /// </summary>
        public string ServiceProviderName { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Distance in meters from the customer's disabled vehicle location
        /// </summary>
        public double Distance { get; set; }
        
        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Zip
        /// </summary>
        public int Zipcode { get; set; }
        
        /// <summary>
        /// County
        /// </summary>
        public string County { get; set; } 
       
    }
}
