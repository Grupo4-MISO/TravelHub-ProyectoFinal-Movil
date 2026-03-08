using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;

namespace App.ViewModels;

public partial class CountryViewModel : BaseViewModel
{
    private ObservableCollection<CountryItem> _countries = [];

    public ObservableCollection<CountryItem> Countries
    {
        get => _countries;
        set => SetProperty(ref _countries, value);
    }

    public ICommand SelectCountryCommand { get; }

    public CountryViewModel()
    {
        LoadCountries();
        SelectCountryCommand = new Command<CountryItem?>(async (country) => await SelectCountry(country));
    }

    private void LoadCountries()
    {
        var currentCode = AppSettingsService.Instance.CurrentCountryCode;
        var allCountries = MockDataService.GetCountries();

        Countries.Clear();
        foreach (var country in allCountries)
        {
            Countries.Add(new CountryItem(country, country.Code == currentCode));
        }
    }

    private async Task SelectCountry(CountryItem? selectedCountry)
    {
        if (selectedCountry == null) return;

        // Actualizar selección
        foreach (var country in Countries)
        {
            country.IsSelected = country.Code == selectedCountry.Code;
        }

        // Guardar configuración
        AppSettingsService.Instance.SetCountry(selectedCountry.Code);

        // Mostrar confirmación
        await Shell.Current.DisplayAlert(
            "País seleccionado",
            $"Ahora estás navegando en {selectedCountry.Name}",
            "OK");

        // Volver atrás
        await Shell.Current.GoToAsync("..");
    }
}

public class CountryItem : BaseViewModel
{
    private int _id;
    public int Id
    {
        get => _id;
        init => SetProperty(ref _id, value);
    }
    private string _name;
    public string Name
    {
        get => _name;
        init => SetProperty(ref _name, value);
    }
    private string _code;
    public string Code
    {
        get => _code;
        init => SetProperty(ref _code, value);
    }
    private string _currencyCode;
    public string CurrencyCode
    {
        get => _currencyCode;
        init => SetProperty(ref _currencyCode, value);
    }
    private string _currencySymbol;
    public string CurrencySymbol
    {
        get => _currencySymbol;
        init => SetProperty(ref _currencySymbol, value);
    }
    private string _flagEmoji;
    public string FlagEmoji
    {
        get => _flagEmoji;
        init => SetProperty(ref _flagEmoji, value);
    }
    private string _phoneCode;
    public string PhoneCode
    {
        get => _phoneCode;
        init => SetProperty(ref _phoneCode, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public CountryItem(Country value, bool isSelected)
    {
        Id = value.Id;
        Name = value.Name;
        Code = value.Code;
        CurrencyCode = value.CurrencyCode;
        CurrencySymbol = value.CurrencySymbol;
        FlagEmoji = value.FlagEmoji;
        PhoneCode = value.PhoneCode;
        IsSelected = isSelected;
    }
}