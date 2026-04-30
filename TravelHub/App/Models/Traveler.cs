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