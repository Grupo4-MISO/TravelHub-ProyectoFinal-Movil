using System;
using System.Collections.Generic;
using System.Text;

namespace App.Models
{
    public class Address
    {
        public string id { get; set; } = string.Empty;
        public string traveler_id { get; set; } = string.Empty;
        public string line1 { get; set; } = string.Empty;
        public string line2 { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string countryCode { get; set; } = string.Empty;
        public string postal_code { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
    public class AddressCreateDTO
    {
        public string line1 { get; set; } = string.Empty;
        public string line2 { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string countryCode { get; set; } = string.Empty;
        public string postal_code { get; set; } = string.Empty;
    }

}
