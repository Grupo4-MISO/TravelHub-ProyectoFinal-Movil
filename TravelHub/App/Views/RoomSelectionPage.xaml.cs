using App.ViewModels;

namespace App.Views;

public partial class RoomSelectionPage : ContentPage
{
    public RoomSelectionPage(RoomSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
