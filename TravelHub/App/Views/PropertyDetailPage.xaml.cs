using App.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace App.Views;

public partial class PropertyDetailPage : ContentPage
{
    private readonly PropertyDetailViewModel _viewModel;

    public PropertyDetailPage(PropertyDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateMap();
    }

    protected override void OnDisappearing()
    {
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        base.OnDisappearing();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PropertyDetailViewModel.Property))
        {
            UpdateMap();
        }
    }

    private void UpdateMap()
    {
        var property = _viewModel.Property;
        if (property == null || (property.Latitude == 0 && property.Longitude == 0))
        {
            return;
        }

        var location = new Location(property.Latitude, property.Longitude);
        PropertyMap.Pins.Clear();
        PropertyMap.Pins.Add(new Pin
        {
            Label = property.Name,
            Address = property.Address,
            Location = location
        });

        PropertyMap.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1)));
    }
}
