namespace App.Models;

public class Room
{
    public string Id { get; set; }
    public string PropertyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxGuests { get; set; }
    public decimal PricePerNight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Features { get; set; } = [];
    public bool IsAvailable { get; set; } = true;
    public string BedType { get; set; } = string.Empty;
    public int RoomCount { get; set; } = 1;
}
