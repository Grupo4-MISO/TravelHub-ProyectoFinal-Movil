using Newtonsoft.Json;

namespace App.Models;

public class AuthLoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}
