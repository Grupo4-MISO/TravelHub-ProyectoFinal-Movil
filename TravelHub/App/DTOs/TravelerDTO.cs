using App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class TravelerDTO : Traveler
    {
        [JsonProperty("addresses")]
        public Address? FirstName { get; set; } = null;
    }
}
