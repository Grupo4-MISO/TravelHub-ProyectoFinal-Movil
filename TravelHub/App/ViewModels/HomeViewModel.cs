using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;

namespace App.ViewModels;

public class HomeViewModel : BaseViewModel
{
    public ObservableCollection<string> PromotionalImages { get; } = [];
    public ObservableCollection<Property> FeaturedProperties { get; } = [];
    public ObservableCollection<string> PopularCities { get; } = [];

    private string _selectedCity = string.Empty;
    public string SelectedCity
    {
        get => _selectedCity;
        set => SetProperty(ref _selectedCity, value);
    }

    private DateTime _checkInDate = DateTime.Today.AddDays(7);
    public DateTime CheckInDate
    {
        get => _checkInDate;
        set
        {
            if (SetProperty(ref _checkInDate, value) && value >= _checkOutDate)
                CheckOutDate = value.AddDays(1);
        }
    }

    private DateTime _checkOutDate = DateTime.Today.AddDays(9);
    public DateTime CheckOutDate
    {
        get => _checkOutDate;
        set => SetProperty(ref _checkOutDate, value);
    }

    private int _adults = 2;
    public int Adults
    {
        get => _adults;
        set { if (SetProperty(ref _adults, value)) OnPropertyChanged(nameof(GuestSummary)); }
    }

    private int _children;
    public int Children
    {
        get => _children;
        set { if (SetProperty(ref _children, value)) OnPropertyChanged(nameof(GuestSummary)); }
    }

    private int _rooms = 1;
    public int Rooms
    {
        get => _rooms;
        set { if (SetProperty(ref _rooms, value)) OnPropertyChanged(nameof(GuestSummary)); }
    }

    private bool _isGuestConfigVisible;
    public bool IsGuestConfigVisible
    {
        get => _isGuestConfigVisible;
        set => SetProperty(ref _isGuestConfigVisible, value);
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

    public HomeViewModel()
    {
        Title = "TravelHub";

        SearchCommand = new Command(OnSearch);
        PropertySelectedCommand = new Command<Property>(OnPropertySelected);
        ToggleGuestConfigCommand = new Command(() => IsGuestConfigVisible = !IsGuestConfigVisible);
        IncrementAdultsCommand = new Command(() => Adults++);
        DecrementAdultsCommand = new Command(() => { if (Adults > 1) Adults--; });
        IncrementChildrenCommand = new Command(() => Children++);
        DecrementChildrenCommand = new Command(() => { if (Children > 0) Children--; });
        IncrementRoomsCommand = new Command(() => Rooms++);
        DecrementRoomsCommand = new Command(() => { if (Rooms > 1) Rooms--; });

        LoadData();
    }

    private void LoadData()
    {
        foreach (var img in MockDataService.GetPromotionalImages())
            PromotionalImages.Add(img);

        foreach (var prop in MockDataService.GetFeaturedProperties())
            FeaturedProperties.Add(prop);

        foreach (var city in MockDataService.GetPopularCities())
            PopularCities.Add(city);
    }

    private async void OnSearch()
    {
        var criteria = new SearchCriteria
        {
            City = SelectedCity,
            CheckIn = CheckInDate,
            CheckOut = CheckOutDate,
            Adults = Adults,
            Children = Children,
            Rooms = Rooms
        };

        var navParams = new Dictionary<string, object> { { "criteria", criteria } };
        await Shell.Current.GoToAsync("SearchResultsPage", navParams);
    }

    private async void OnPropertySelected(Property? property)
    {
        if (property == null) return;
        var navParams = new Dictionary<string, object> { { "property", property } };
        await Shell.Current.GoToAsync("PropertyDetailPage", navParams);
    }
}
