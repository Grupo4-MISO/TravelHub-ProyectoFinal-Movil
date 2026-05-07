using App.ViewModels;

namespace App.Views;

public partial class PaymentPage : ContentPage
{
    private readonly PaymentViewModel _viewModel;

    public PaymentPage(PaymentViewModel viewModel)
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
