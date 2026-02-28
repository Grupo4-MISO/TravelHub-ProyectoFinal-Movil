namespace App.Models;

public class Traveler
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentType { get; set; } = "DNI";
    public string DocumentNumber { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
}
