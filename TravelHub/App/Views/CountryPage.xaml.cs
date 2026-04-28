using App.ViewModels;

namespace App.Views;

public partial class CountryPage : ContentPage
{
    public CountryPage(CountryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Actualizar visibilidad del mensaje de error cuando aparece la página
        if (BindingContext is CountryViewModel vm)
        {
            UpdateErrorVisibility();
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CountryViewModel.ErrorMessage))
                {
                    UpdateErrorVisibility();
                }
            };
        }
    }

    private void UpdateErrorVisibility()
    {
        if (BindingContext is CountryViewModel vm)
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