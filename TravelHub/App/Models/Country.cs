using SQLite;

namespace App.Models;

[Table("Country")]
public class Country
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [Indexed]
    public string Name { get; set; } = string.Empty;

    [Indexed]
    public string Code { get; set; } = string.Empty; // CO, PE, EC, MX, CL, AR

    public string CurrencyCode { get; set; } = string.Empty; // COP, PEN, USD, MXN, CLP, ARS
    public string CurrencySymbol { get; set; } = string.Empty; // $, S/, $, $, $, $
    public string FlagEmoji { get; set; } = string.Empty; // 🇨🇴, 🇵🇪, 🇪🇨, 🇲🇽, 🇨🇱, 🇦🇷
    public string PhoneCode { get; set; } = string.Empty; // +57, +51, +593, +52, +56, +54
}
