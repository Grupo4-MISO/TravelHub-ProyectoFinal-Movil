using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class AccommodationDetailAmenityDto
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;
    }

}
