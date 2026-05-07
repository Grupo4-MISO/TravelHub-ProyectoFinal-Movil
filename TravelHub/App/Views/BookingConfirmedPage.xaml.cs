using App.ViewModels;

namespace App.Views;

public partial class BookingConfirmedPage : ContentPage
{
    public BookingConfirmedPage(BookingConfirmedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
