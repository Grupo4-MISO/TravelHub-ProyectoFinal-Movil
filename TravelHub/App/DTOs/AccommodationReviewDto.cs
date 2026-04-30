using Newtonsoft.Json;

namespace App.DTOs;

public class AccommodationReviewDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("hospedajeId")]
    public string PropertyId { get; set; } = string.Empty;

    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonProperty("rating")]
    public double Rating { get; set; }
}
