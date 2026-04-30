using App.ViewModels;

namespace App.Views;

public partial class ActiveBookingsPage : ContentPage
{
    public ActiveBookingsPage(ActiveBookingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
