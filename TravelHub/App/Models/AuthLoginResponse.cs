using Newtonsoft.Json;

namespace App.Models;

public class AuthLoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;

    [JsonProperty("user")]
    public AuthUserDto User { get; set; } = new();
}

public class AuthUserDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("role")]
    public string Role { get; set; } = string.Empty;
}
