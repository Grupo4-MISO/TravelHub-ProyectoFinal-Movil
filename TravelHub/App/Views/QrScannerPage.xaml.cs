using App.Services.Interfaces;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
namespace App.Views;

public partial class QrScannerPage : ContentPage
{
    private readonly IQrScanResultService _qrScanResultService;

    public QrScannerPage(IQrScanResultService qrScanResultService)
    {
        InitializeComponent();
        _qrScanResultService = qrScanResultService;

        cameraView.Options = new BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _qrScanResultService.Clear();
        cameraView.IsEnabled = true;
        cameraView.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraView.IsDetecting = false;
        cameraView.IsEnabled = false;
    }

    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var first = e.Results?.FirstOrDefault();
        if (first?.Value is null)
            return;

        cameraView.IsDetecting = false;
        _qrScanResultService.ScannedUrl = first.Value;
        await Shell.Current.GoToAsync("..");
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        cameraView.IsDetecting = false;
        await Shell.Current.GoToAsync("..");
    }
}
