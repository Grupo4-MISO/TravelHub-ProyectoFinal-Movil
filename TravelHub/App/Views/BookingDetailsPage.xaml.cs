using App.ViewModels;

namespace App.Views;

public partial class BookingDetailsPage : ContentPage
{
    public BookingDetailsPage(BookingDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}