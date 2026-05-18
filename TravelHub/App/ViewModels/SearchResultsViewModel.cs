using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services.Interfaces;
using App.Services.Implementations;

namespace App.ViewModels;

public class SearchResultsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAccommodationSearchService _accommodationSearchService;
    private readonly ILocalizationService _localizationService;
    private readonly List<SearchAccommodationDto> _allProperties = [];

    public ObservableCollection<SearchAccommodationDto> Properties { get; } = [];

    public ObservableCollection<string> SortOptions { get; } = [];

    private SearchCriteria _criteria = new();
    public SearchCriteria Criteria
    {
        get => _criteria;
        set => SetProperty(ref _criteria, value);
    }

    private string _sortBy = string.Empty;
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

    private double _maxAvailablePrice = 500;
    public double MaxAvailablePrice
    {
        get => _maxAvailablePrice;
        set => SetProperty(ref _maxAvailablePrice, value);
    }

    private double _minRating;
    public double MinRating
    {
        get => _minRating;
        set => SetProperty(ref _minRating, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ICommand PropertySelectedCommand { get; }
    public ICommand ToggleFilterCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }

    public SearchResultsViewModel(IAccommodationSearchService accommodationSearchService, ILocalizationService localizationService)
    {
        _accommodationSearchService = accommodationSearchService ?? throw new ArgumentNullException(nameof(accommodationSearchService));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        
        Title = "Resultados";
        
        // Initialize sort options with localized strings
        SortOptions.Add(_localizationService.GetString("Search_Recomendados"));
        SortOptions.Add(_localizationService.GetString("Search_PrecioMenor"));
        SortOptions.Add(_localizationService.GetString("Search_PrecioMayor"));
        SortOptions.Add(_localizationService.GetString("Search_MejorCalificado"));
        
        SortBy = SortOptions.First();
        
        PropertySelectedCommand = new Command<SearchAccommodationDto>(OnPropertySelected);
        ToggleFilterCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
        ApplyFilterCommand = new Command(OnApplyFilter);
        ClearFilterCommand = new Command(OnClearFilter);
        
        _localizationService.LanguageChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var currentSort = SortBy;
                SortOptions.Clear();
                SortOptions.Add(_localizationService.GetString("Search_Recomendados"));
                SortOptions.Add(_localizationService.GetString("Search_PrecioMenor"));
                SortOptions.Add(_localizationService.GetString("Search_PrecioMayor"));
                SortOptions.Add(_localizationService.GetString("Search_MejorCalificado"));
                
                // Try to find the equivalent option in new language
                SortBy = SortOptions.FirstOrDefault() ?? string.Empty;
            });
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("criteria", out var obj) && obj is SearchCriteria criteria)
        {
            Criteria = criteria;
            Title = string.IsNullOrEmpty(criteria.City) ? "Resultados" : $"Hoteles en {criteria.City}";
        }
        _ = LoadPropertiesAsync();
    }

    private async Task LoadPropertiesAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            ErrorMessage = string.Empty;
            Properties.Clear();
            _allProperties.Clear();

            var response = await _accommodationSearchService.SearchAccommodationsAsync(Criteria);
            if (response.Error || response.Response == null)
            {
                ErrorMessage = await response.GetErrorMessageAsync();
                return;
            }

            _allProperties.AddRange(response.Response);
            if (_allProperties.Count == 0)
            {
                ErrorMessage = "No se encontraron alojamientos para esta búsqueda.";
                return;
            }

            MaxAvailablePrice = Math.Ceiling((double)_allProperties.Max(property => property.Price));
            MaxPrice = MaxAvailablePrice;
            ApplySortAndFilters();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplySortAndFilters()
    {
        var filtered = _allProperties
            .Where(p => (double)p.Price >= MinPrice && (double)p.Price <= MaxPrice)
            .Where(p => p.Rating >= MinRating);

        var sorted = SortBy switch
        {
            var s when s == _localizationService.GetString("Search_PrecioMenor") => filtered.OrderBy(p => p.Price),
            var s when s == _localizationService.GetString("Search_PrecioMayor") => filtered.OrderByDescending(p => p.Price),
            var s when s == _localizationService.GetString("Search_MejorCalificado") => filtered.OrderByDescending(p => p.Rating),
            _ => filtered
        };

        Properties.Clear();
        foreach (var property in sorted)
        {
            Properties.Add(property);
        }
    }

    private void ApplySort()
    {
        ApplySortAndFilters();
    }

    private void OnApplyFilter()
    {
        ApplySortAndFilters();
        IsFilterVisible = false;
    }

    private void OnClearFilter()
    {
        MinPrice = 0;
        MaxPrice = MaxAvailablePrice;
        MinRating = 0;
        ApplySortAndFilters();
        IsFilterVisible = false;
    }

    private async void OnPropertySelected(SearchAccommodationDto? property)
    {
        if (property == null || IsBusy) return;
        IsBusy = true;
        try
        {
            var navParams = new Dictionary<string, object> { { "property", property } };
            await Shell.Current.GoToAsync("PropertyDetailPage", navParams);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error navigating to PropertyDetail: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
