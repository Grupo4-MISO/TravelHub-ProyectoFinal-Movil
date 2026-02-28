namespace App.Models;

public class SearchCriteria
{
    public string City { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(7);
    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(9);
    public int Adults { get; set; } = 2;
    public int Children { get; set; }
    public int Rooms { get; set; } = 1;

    public int Nights => (CheckOut - CheckIn).Days;
}
