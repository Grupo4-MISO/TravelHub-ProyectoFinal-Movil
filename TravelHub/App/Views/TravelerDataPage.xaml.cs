using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class TravelerDataPage : ContentPage
{
    private readonly TravelerDataViewModel _viewModel;

    public TravelerDataPage()
    {
        InitializeComponent();
        _viewModel = IPlatformApplication.Current?.Services.GetRequiredService<TravelerDataViewModel>()
            ?? throw new InvalidOperationException("No se pudo resolver TravelerDataViewModel.");
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.EnsureAuthenticatedAndLoadTravelerAsync();
    }
}
