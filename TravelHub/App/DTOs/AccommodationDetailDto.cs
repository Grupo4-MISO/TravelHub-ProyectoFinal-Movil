using Newtonsoft.Json;

namespace App.DTOs;

public class AccommodationDetailDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("providerId")]
    public string ProviderId { get; set; } = string.Empty;

    [JsonProperty("nombre")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("descripcion")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonProperty("pais")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("ciudad")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("direccion")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("rating")]
    public double Rating { get; set; }

    [JsonProperty("reviews")]
    public int Reviews { get; set; }

    [JsonProperty("habitaciones")]
    public List<AccommodationDetailRoomDto> Rooms { get; set; } = [];

    [JsonProperty("amenidades")]
    public List<AccommodationDetailAmenityDto> Amenities { get; set; } = [];

    [JsonProperty("imagenes")]
    public List<AccommodationDetailImageDto> Images { get; set; } = [];
}
