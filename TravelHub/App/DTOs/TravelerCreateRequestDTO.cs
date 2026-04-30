using App.Models;
using Newtonsoft.Json;

namespace App.DTOs;

public class TravelerCreateRequestDTO
{

    [JsonProperty("traveler")]
    public TravelerCreateDTO traveler { get; set; } = new TravelerCreateDTO();

    [JsonProperty("address")]
    public AddressCreateDTO address { get; set; } = new AddressCreateDTO();

}
