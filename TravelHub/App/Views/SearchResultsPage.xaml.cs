using App.ViewModels;

namespace App.Views;

public partial class SearchResultsPage : ContentPage
{
    public SearchResultsPage(SearchResultsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
