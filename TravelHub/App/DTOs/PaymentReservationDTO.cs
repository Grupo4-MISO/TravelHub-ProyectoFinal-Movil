using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace App.DTOs;

public class PaymentReservationDTO
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
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("provider_payment_id")]
    public string? ProviderPaymentId { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonIgnore]
    public JsonObject? Metadata { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
