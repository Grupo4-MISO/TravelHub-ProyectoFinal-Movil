using System;
using System.Collections.Generic;
using System.Text;

namespace App.Models
{
    public class Address
    {
        public string id { get; set; }
        public string traveler_id { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string postal_code { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
    public class AddressCreateDTO
    {
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string postal_code { get; set; }
    }

}
