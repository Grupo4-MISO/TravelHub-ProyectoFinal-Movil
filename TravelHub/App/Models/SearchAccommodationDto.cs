using Newtonsoft.Json;

namespace App.Models;

public class SearchAccommodationDto
{
    [JsonProperty("habitacion_id")]
    public string RoomId { get; set; } = string.Empty;

    [JsonProperty("hospedaje_id")]
    public string PropertyId { get; set; } = string.Empty;

    [JsonProperty("code")]
    public string RoomCode { get; set; } = string.Empty;

    [JsonProperty("nombre")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("pais")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("ciudad")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("direccion")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("rating")]
    public double Rating { get; set; }

    [JsonProperty("reviews")]
    public int Reviews { get; set; }

    [JsonProperty("capacidad")]
    public int Capacity { get; set; }

    [JsonProperty("precio")]
    public decimal Price { get; set; }

    [JsonProperty("descripcion")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; } = string.Empty;
}
