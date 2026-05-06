namespace App.Models;

public class Reservation
{
    public string Id { get; set; } = string.Empty;
    public string BookingCode { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string Status { get; set; } = "Confirmada";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Hotel details (populated from hotel inventory API)
    public string HotelName { get; set; } = string.Empty;
    public string HotelCity { get; set; } = string.Empty;
    public string HotelCountry { get; set; } = string.Empty;
    public string HotelAddress { get; set; } = string.Empty;
    public string HotelImage { get; set; } = string.Empty;

    public int Nights => (CheckOut - CheckIn).Days;
}
