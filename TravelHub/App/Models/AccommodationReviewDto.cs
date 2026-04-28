using Newtonsoft.Json;

namespace App.Models;

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
    public int Rating { get; set; }
}
