using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;

namespace App.ViewModels;

public class SearchResultsViewModel : BaseViewModel, IQueryAttributable
{
    public ObservableCollection<Property> Properties { get; } = [];

    private SearchCriteria _criteria = new();
    public SearchCriteria Criteria
    {
        get => _criteria;
        set => SetProperty(ref _criteria, value);
    }

    private string _sortBy = "Recomendados";
    public string SortBy
    {
        get => _sortBy;
        set
        {
            if (SetProperty(ref _sortBy, value))
                ApplySort();
        }
    }

    private bool _isFilterVisible;
    public bool IsFilterVisible
    {
        get => _isFilterVisible;
        set => SetProperty(ref _isFilterVisible, value);
    }

    private double _minPrice;
    public double MinPrice
    {
        get => _minPrice;
        set => SetProperty(ref _minPrice, value);
    }

    private double _maxPrice = 500;
    public double MaxPrice
    {
        get => _maxPrice;
        set => SetProperty(ref _maxPrice, value);
    }

    private double _minRating;
    public double MinRating
    {
        get => _minRating;
        set => SetProperty(ref _minRating, value);
    }

    public ICommand PropertySelectedCommand { get; }
    public ICommand ToggleFilterCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }

    public SearchResultsViewModel()
    {
        Title = "Resultados";
        PropertySelectedCommand = new Command<Property>(OnPropertySelected);
        ToggleFilterCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
        ApplyFilterCommand = new Command(OnApplyFilter);
        ClearFilterCommand = new Command(OnClearFilter);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("criteria", out var obj) && obj is SearchCriteria criteria)
        {
            Criteria = criteria;
            Title = string.IsNullOrEmpty(criteria.City) ? "Resultados" : $"Hoteles en {criteria.City}";
        }
        LoadProperties();
    }

    private void LoadProperties()
    {
        Properties.Clear();
        var allProperties = MockDataService.GetFeaturedProperties();

        var filtered = string.IsNullOrEmpty(Criteria.City)
            ? allProperties
            : allProperties.Where(p => p.City.Contains(Criteria.City, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!filtered.Any())
            filtered = allProperties;

        foreach (var prop in filtered)
            Properties.Add(prop);
    }

    private void ApplySort()
    {
        var sorted = SortBy switch
        {
            "Precio menor" => Properties.OrderBy(p => p.PricePerNight).ToList(),
            "Precio mayor" => Properties.OrderByDescending(p => p.PricePerNight).ToList(),
            "Mejor calificado" => Properties.OrderByDescending(p => p.Rating).ToList(),
            _ => Properties.ToList()
        };

        Properties.Clear();
        foreach (var p in sorted)
            Properties.Add(p);
    }

    private void OnApplyFilter()
    {
        var allProperties = MockDataService.GetFeaturedProperties();
        var filtered = allProperties
            .Where(p => (double)p.PricePerNight >= MinPrice && (double)p.PricePerNight <= MaxPrice)
            .Where(p => p.Rating >= MinRating)
            .ToList();

        Properties.Clear();
        foreach (var p in filtered)
            Properties.Add(p);

        IsFilterVisible = false;
    }

    private void OnClearFilter()
    {
        MinPrice = 0;
        MaxPrice = 500;
        MinRating = 0;
        LoadProperties();
        IsFilterVisible = false;
    }

    private async void OnPropertySelected(Property? property)
    {
        if (property == null) return;
        var navParams = new Dictionary<string, object> { { "property", property } };
        await Shell.Current.GoToAsync("PropertyDetailPage", navParams);
    }
}
