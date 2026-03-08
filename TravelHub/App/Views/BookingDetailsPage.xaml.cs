using App.ViewModels;

namespace App.Views;

public partial class BookingDetailsPage : ContentPage
{
    public BookingDetailsPage()
    {
        InitializeComponent();
        BindingContext = new BookingDetailsViewModel();
    }
}