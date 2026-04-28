using Newtonsoft.Json;

namespace App.Models;

public class TravelerUpdateRequest
{
    [JsonProperty("documentNumber")]
    public string DocumentNumber { get; set; } = string.Empty;

    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("gender")]
    public string Gender { get; set; } = "Female";

    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;
}
