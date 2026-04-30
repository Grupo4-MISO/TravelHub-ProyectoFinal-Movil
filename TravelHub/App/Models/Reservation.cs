namespace App.Models;

public class Reservation
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    //public Property Property { get; set; } = new();
    public Room Room { get; set; } = new();
    public Traveler Traveler { get; set; } = new();
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Adults { get; set; } = 1;
    public int Children { get; set; }
    public int RoomCount { get; set; } = 1;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Confirmada";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int Nights => (CheckOut - CheckIn).Days;
}
