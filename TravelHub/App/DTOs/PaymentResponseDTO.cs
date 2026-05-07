using Newtonsoft.Json;
using OneSignalSDK.DotNet.Core.Internal.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace App.DTOs
{
    public class PaymentResponseDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("reserva_id")]
        public string ReservaId { get; set; } = string.Empty;

        [JsonProperty("provider_id")]
        public string ProviderId { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;  // Ejemplo: "USD", "EUR"

        [JsonProperty("status")]
        public string? Estado { get; set; } = string.Empty;

        [JsonProperty("provider_payment_id")]
        public string? ProviderPaymentId { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string? Url { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; } = string.Empty;

        [JsonIgnore]
        public JsonObject? Metadata { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

    }
}
