using Newtonsoft.Json;

namespace App.DTOs;

public class PopularAccommodationsDto
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

    [JsonProperty("rating")]
    public double Rating { get; set; }

    [JsonProperty("reviews")]
    public int Reviews { get; set; }

    [JsonProperty("precio")]
    public decimal Price { get; set; }

    [JsonProperty("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; } = string.Empty;
}
