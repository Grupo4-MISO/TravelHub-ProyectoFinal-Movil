using Newtonsoft.Json;

namespace App.Models;

public class Traveler
{
    public string id { get; set; }
    public string documentNumber { get; set; }
    public string userId { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string phone { get; set; }
    public string gender { get; set; }
    public string photo { get; set; }
    public string email { get; set; }
    public string travelerStatus { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

}

public class TravelerDTO : Traveler
{

    [JsonProperty("addresses")]
    public Address? FirstName { get; set; } = null;
}

public class TravelerCreateDTO
{
    public string documentNumber { get; set; } = string.Empty;
    public string first_name { get; set; } = string.Empty;
    public string last_name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string gender { get; set; } = string.Empty;
}
