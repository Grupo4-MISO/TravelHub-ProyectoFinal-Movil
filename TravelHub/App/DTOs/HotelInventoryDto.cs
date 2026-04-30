using Newtonsoft.Json;

namespace App.DTOs;

public class HotelInventoryDto
{
    [JsonProperty("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonProperty("pais")]
    public string Pais { get; set; } = string.Empty;

    [JsonProperty("ciudad")]
    public string Ciudad { get; set; } = string.Empty;

    [JsonProperty("direccion")]
    public string Direccion { get; set; } = string.Empty;

    [JsonProperty("imagen")]
    public string Imagen { get; set; } = string.Empty;
}
