using Newtonsoft.Json;

namespace App.Models;

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

public class AccommodationDetailAmenityDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class AccommodationDetailImageDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
