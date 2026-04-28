using Newtonsoft.Json;

namespace App.Models;

public class AppConfigResponse
{
    [JsonProperty("AppName")]
    public string AppName { get; set; } = string.Empty;

    [JsonProperty("AppBackEnd")]
    public string AppBackEnd { get; set; } = string.Empty;
}
