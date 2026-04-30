namespace App.DTOs;

public class AddressCreateDTO
{
    public string city { get; set; } = string.Empty;
    public string country { get; set; } = string.Empty;
    public string countryCode { get; set; } = string.Empty;
    public string line1 { get; set; } = string.Empty;
    public string line2 { get; set; } = string.Empty;
    public string postal_code { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
}
