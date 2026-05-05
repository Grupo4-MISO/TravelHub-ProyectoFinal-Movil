using Newtonsoft.Json;

namespace App.DTOs;

public class ReservationHoldRequestDto
{
    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("habitacion_id")]
    public string HabitacionId { get; set; } = string.Empty;

    [JsonProperty("check_in")]
    public string CheckIn { get; set; } = string.Empty;

    [JsonProperty("check_out")]
    public string CheckOut { get; set; } = string.Empty;
}
