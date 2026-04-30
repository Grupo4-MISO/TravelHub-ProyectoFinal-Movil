using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class AccommodationDetailRoomDto
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("descripcion")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("capacidad")]
        public int Capacity { get; set; }

        [JsonProperty("precio")]
        public decimal Price { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
