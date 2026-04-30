using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class TravelerCreateDTO
    {
        public string documentNumber { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string photo { get; set; } = string.Empty;
        public string travelerStatus { get; set; } = "Pending";
    }

}
