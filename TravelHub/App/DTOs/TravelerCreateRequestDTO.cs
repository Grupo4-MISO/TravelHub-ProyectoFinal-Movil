using App.Models;
using Newtonsoft.Json;

namespace App.DTOs;

public class TravelerCreateRequestDTO
{

    [JsonProperty("traveler")]
    public TravelerCreateDTO traveler { get; set; }

    [JsonProperty("addresses")]
    public AddressCreateDTO address { get; set; }

}
