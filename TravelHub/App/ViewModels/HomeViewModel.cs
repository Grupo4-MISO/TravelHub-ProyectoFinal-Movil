using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Providers.Interfaces;
using App.Services;
using App.Services.Interfaces;

namespace App.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly ICountryService _countryService;
    private readonly ICityService _cityService;
    private readonly IBackendUrlProvider _backendUrlProvider;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IAppConfigurationService _appConfigurationService;

    public ObservableCollection<string> PromotionalImages { get; } = [];
    public ObservableCollection<SearchAccommodationDto> FeaturedProperties { get; } = [];
    public ObservableCollection<string> PopularCities { get; } = [];

    private string _selectedCity = string.Empty;
    public string SelectedCity
    {
        get => _selectedCity;
        set => SetProperty(ref _selectedCity, value);
    }

    private DateTime _checkInDate = DateTime.Today.AddDays(1);
    public DateTime CheckInDate
    {
        get => _checkInDate;
        set
        {
            if (SetProperty(ref _checkInDate, value) && value >= _checkOutDate)
            {
                CheckOutDate = value.AddDays(1);
            }
        }
    }

    private DateTime _checkOutDate = DateTime.Today.AddDays(3);
    public DateTime CheckOutDate
    {
        get => _checkOutDate;
        set => SetProperty(ref _checkOutDate, value);
    }

    private int _adults = 2;
    public int Adults
    {
        get => _adults;
        set
        {
            if (SetProperty(ref _adults, value))
            {
                OnPropertyChanged(nameof(GuestSummary));
            }
        }
    }

    private int _children;
    public int Children
    {
        get => _children;
        set
        {
            if (SetProperty(ref _children, value))
            {
                OnPropertyChanged(nameof(GuestSummary));
            }
        }
    }

    private int _rooms = 1;
    public int Rooms
    {
        get => _rooms;
        set
        {
            if (SetProperty(ref _rooms, value))
            {
                OnPropertyChanged(nameof(GuestSummary));
            }
        }
    }

    private bool _isGuestConfigVisible;
    public bool IsGuestConfigVisible
    {
        get => _isGuestConfigVisible;
        set => SetProperty(ref _isGuestConfigVisible, value);
    }

    private string _popularCitiesErrorMessage = string.Empty;
    public string PopularCitiesErrorMessage
    {
        get => _popularCitiesErrorMessage;
        set => SetProperty(ref _popularCitiesErrorMessage, value);
    }
    public string GuestSummary
    {
        get
        {
            var parts = new List<string>
            {
                $"{Rooms} habitacion{(Rooms > 1 ? "es" : "")}",
                $"{Adults} Adulto{(Adults > 1 ? "s" : "")}"
            };
            parts.Add(Children > 0 ? $"{Children} Nino{(Children > 1 ? "s" : "")}" : "Sin ninos");
            return string.Join(" - ", parts);
        }
    }

    public DateTime MinDate => DateTime.Today;
    public ICommand SearchCommand { get; }
    public ICommand PropertySelectedCommand { get; }
    public ICommand ToggleGuestConfigCommand { get; }
    public ICommand IncrementAdultsCommand { get; }
    public ICommand DecrementAdultsCommand { get; }
    public ICommand IncrementChildrenCommand { get; }
    public ICommand DecrementChildrenCommand { get; }
    public ICommand IncrementRoomsCommand { get; }
    public ICommand DecrementRoomsCommand { get; }

    public HomeViewModel(ICountryService countryService, ICityService cityService, IBackendUrlProvider backendUrlProvider, IAppSettingsService appSettingsService, IAppConfigurationService appConfigurationService)
    {
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        _appConfigurationService = appConfigurationService ?? throw new ArgumentNullException(nameof(appConfigurationService));
        Title = "TravelHub";

        SearchCommand = new Command(OnSearch);
        ToggleGuestConfigCommand = new Command(() => IsGuestConfigVisible = !IsGuestConfigVisible);
        IncrementAdultsCommand = new Command(() => Adults++);
        DecrementAdultsCommand = new Command(() => { if (Adults > 1) { Adults--; } });
        IncrementChildrenCommand = new Command(() => Children++);
        DecrementChildrenCommand = new Command(() => { if (Children > 0) { Children--; } });
        IncrementRoomsCommand = new Command(() => Rooms++);
        DecrementRoomsCommand = new Command(() => { if (Rooms > 1) { Rooms--; } });

        _ = LoadDataAsync();
        _appSettingsService.CountryChanged += OnCountryChanged;
        _backendUrlProvider.BaseUrlChanged += OnBackendUrlChanged;
    }

    private async Task LoadDataAsync()
    {
        var currentCountryCode = _appSettingsService.CurrentCountryCode;

        PromotionalImages.Clear();
        var images = await _appConfigurationService.GetPromotionalImagesAsync();
        foreach (var img in images)
        {
            PromotionalImages.Add(img);
        }

        await LoadPopularCitiesAsync(currentCountryCode);
        SelectedCity = PopularCities.FirstOrDefault() ?? string.Empty;
    }

    private async Task LoadPopularCitiesAsync(string countryCode)
    {
        PopularCitiesErrorMessage = string.Empty;
        PopularCities.Clear();

        var citiesResponse = await _cityService.GetPopularCitiesByCountryAsync(countryCode);
        if (citiesResponse.Error || citiesResponse.Response == null || citiesResponse.Response.Count == 0)
        {
            PopularCitiesErrorMessage = "No se pudieron cargar ciudades populares para el pais seleccionado.";
            return;
        }

        foreach (var city in citiesResponse.Response)
        {
            PopularCities.Add(city);
        }
    }

    private void OnCountryChanged(object? sender, string countryCode)
    {
        _ = LoadDataAsync();
    }

    private void OnBackendUrlChanged(object? sender, string newBaseUrl)
    {
        _ = LoadDataAsync();
    }

    private async void OnSearch()
    {
        if (string.IsNullOrWhiteSpace(SelectedCity))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Por favor, seleccione una ciudad.", "OK");
            return;
        }

        var countryCode = _appSettingsService.CurrentCountryCode;

        var criteria = new SearchCriteria
        {
            City = SelectedCity,
            CheckIn = CheckInDate,
            CheckOut = CheckOutDate,
            Adults = Adults,
            Children = Children,
            Rooms = Rooms,
            CountryCode = string.IsNullOrWhiteSpace(countryCode) ? "CO" : countryCode.Trim().ToUpperInvariant(),
            CurrencyCode = _appSettingsService.CurrentCurrencyCode
        };

        var navParams = new Dictionary<string, object> { { "criteria", criteria } };
        await Shell.Current.GoToAsync("SearchResultsPage", navParams);
    }

    private async void OnPropertySelected(SearchAccommodationDto? property)
    {
        if (property == null)
        {
            return;
        }

        var navParams = new Dictionary<string, object> { { "property", property } };
        await Shell.Current.GoToAsync("PropertyDetailPage", navParams);
    }
}
