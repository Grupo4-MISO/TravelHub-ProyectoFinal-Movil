using Newtonsoft.Json;

namespace App.DTOs;

public class CreateReservationRequest
{
    public ReservationCreateResponseDto Reserva { get; set; }
}

public class ReservationCreateResponseDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("public_id")]
    public string BookingCode { get; set; } = string.Empty;

    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("habitacion_id")]
    public string HabitacionId { get; set; } = string.Empty;

    [JsonProperty("check_in")]
    public string CheckIn { get; set; } = string.Empty;

    [JsonProperty("check_out")]
    public string CheckOut { get; set; } = string.Empty;

    [JsonProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.MinValue;

    [JsonProperty("fecha_creacion")]
    public string FechaCreacion { get; set; } = string.Empty;

    [JsonProperty("fecha_actualizacion")]
    public string FechaActualizacion { get; set; } = string.Empty;
}
