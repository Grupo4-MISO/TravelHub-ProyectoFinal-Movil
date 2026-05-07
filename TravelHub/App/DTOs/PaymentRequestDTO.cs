using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class PaymentRequestDTO
    {
        [JsonProperty("reserva_id")]
        public string ReservaId { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;  // Ejemplo: "USD", "EUR"

        [JsonProperty("provider_id")]
        public string ProviderId { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
