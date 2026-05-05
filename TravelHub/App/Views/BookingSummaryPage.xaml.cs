using App.ViewModels;

namespace App.Views;

public partial class BookingSummaryPage : ContentPage
{
    private readonly BookingSummaryViewModel _viewModel;

    public BookingSummaryPage(BookingSummaryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.EnsureReservationHoldAsync();
    }
}
