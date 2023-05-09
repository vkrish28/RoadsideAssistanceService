﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Models
{
    /// <summary>
    /// Customer business model
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
    }
}
