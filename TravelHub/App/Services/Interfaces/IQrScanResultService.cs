namespace App.Services.Interfaces;

public interface IQrScanResultService
{
    string? ScannedUrl { get; set; }
    void Clear();
}
