using App.Services.Interfaces;

namespace App.Services.Implementations;

public class QrScanResultService : IQrScanResultService
{
    public string? ScannedUrl { get; set; }

    public void Clear()
    {
        ScannedUrl = null;
    }
}
