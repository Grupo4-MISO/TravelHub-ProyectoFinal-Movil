using Newtonsoft.Json;

namespace App.DTOs;

public class BookingResponseDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("public_id")]
    public string PublicId { get; set; } = string.Empty;

    [JsonProperty("habitacion_id")]
    public string HabitacionId { get; set; } = string.Empty;

    [JsonProperty("check_in")]
    public string CheckIn { get; set; } = string.Empty;

    [JsonProperty("check_out")]
    public string CheckOut { get; set; } = string.Empty;

    [JsonProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
