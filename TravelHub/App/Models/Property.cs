namespace App.Models;

public class Property
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public decimal PricePerNight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = [];
    public List<Amenity> Amenities { get; set; } = [];
    public bool IsAvailable { get; set; } = true;
    public List<Room> Rooms { get; set; } = [];
    public List<Review> Reviews { get; set; } = [];
    public string PropertyType { get; set; } = "Hotel";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Country Country { get; set; } = new(); // Nueva propiedad
}

public class Review
{
    public string AuthorName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class Amenity
{
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}