using Newtonsoft.Json;

namespace App.Models;

public class Traveler
{
    public string id { get; set; } = string.Empty;
    public string documentNumber { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;
    public string first_name { get; set; } = string.Empty;
    public string last_name { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string gender { get; set; } = string.Empty;
    public string photo { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string travelerStatus { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

}