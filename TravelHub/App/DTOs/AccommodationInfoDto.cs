using Newtonsoft.Json;

namespace App.DTOs;

public class AccommodationInfoDto
{

    [JsonProperty("nombre")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("direccion")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("ciudad")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("pais")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("habitacion")]
    public AccommodationDetailRoomDto? Room { get; set; } 

    [JsonProperty("amenidades")]
    public List<AccommodationDetailAmenityDto>? Amenities { get; set; } = [];

    [JsonProperty("imagenes")]
    public List<AccommodationDetailImageDto>? Images { get; set; } = [];
}
