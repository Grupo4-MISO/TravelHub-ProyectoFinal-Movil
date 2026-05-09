using App.ViewModels;

namespace App.Views;

public partial class CurrencyPage : ContentPage
{
    public CurrencyPage(CurrencyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CurrencyViewModel vm)
        {
            UpdateErrorVisibility();
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CurrencyViewModel.ErrorMessage))
                {
                    UpdateErrorVisibility();
                }
            };
        }
    }

    private void UpdateErrorVisibility()
    {
        if (BindingContext is CurrencyViewModel vm)
        {
            var errorLayout = this.FindByName<VerticalStackLayout>("ErrorLayout") ??
                            (this.FindByName("ErrorLayout") as VerticalStackLayout);

            if (errorLayout != null)
            {
                errorLayout.IsVisible = !string.IsNullOrEmpty(vm.ErrorMessage);
            }
        }
    }
}
