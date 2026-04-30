using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class AccommodationDetailImageDto
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
    }
}
