using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace App.DTOs;

public class PaymentProviderDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("config")]
    public PaymentProviderConfigDto? Config { get; set; }

    [JsonProperty("is_active")]
    public Boolean IsActive { get; set; }

    [JsonProperty("logo")]
    public string Logo { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class PaymentProviderConfigDto
{
    [JsonProperty("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonProperty("secretKey")]
    public string SecretKey { get; set; } = string.Empty;
}
