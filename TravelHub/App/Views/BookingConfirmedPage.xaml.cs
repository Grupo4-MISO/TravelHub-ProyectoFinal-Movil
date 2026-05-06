using App.ViewModels;

namespace App.Views;

public partial class BookingConfirmedPage : ContentPage
{
    private readonly BookingConfirmedViewModel _viewModel;

    public BookingConfirmedPage(BookingConfirmedViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.EnsurePaymentProvidersLoadedAsync();
    }
}
