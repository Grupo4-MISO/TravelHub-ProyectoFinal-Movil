using Newtonsoft.Json;

namespace App.Models;

public class TravelerCreateRequest
{

    [JsonProperty("traveler")]
    public TravelerCreateDTO traveler { get; set; }

    [JsonProperty("addresses")]
    public AddressCreateDTO address { get; set; }

}
