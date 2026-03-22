using App.Models;
using App.Services;
using System.Windows.Input;

namespace App.ViewModels;

public class TravelerDataViewModel : BaseViewModel, IQueryAttributable
{
    private Property _property = new();
    public Property Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    private Room _room = new();
    public Room Room
    {
        get => _room;
        set => SetProperty(ref _room, value);
    }

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }
    private string _phoneNumber = string.Empty; // Número de teléfono sin código de país
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }
    private string _phoneCode = string.Empty; // Código del país actual
    public string PhoneCode
    {
        get => _phoneCode;
        set => SetProperty(ref _phoneCode, value);
    }

    private string countryFlag = string.Empty; // Bandera del país
    public string CountryFlag
    {
        get => countryFlag;
        set => SetProperty(ref countryFlag, value);
    }

    private string _documentNumber = string.Empty;
    public string DocumentNumber
    {
        get => _documentNumber;
        set => SetProperty(ref _documentNumber, value);
    }
    public string FullPhoneNumber => $"{PhoneCode} {PhoneNumber}".Trim();
    public ICommand ContinueCommand { get; }

    public TravelerDataViewModel()
    {
        Title = "Datos del Viajero";
        LoadPhoneCodeFromCurrentCountry();
        ContinueCommand = new Command(OnContinue);
        // Suscribirse a cambios de país
        AppSettingsService.Instance.CountryChanged += OnCountryChanged;
    }

    private void LoadPhoneCodeFromCurrentCountry()
    {
        var country = AppSettingsService.Instance.CurrentCountry;
        PhoneCode = country.PhoneCode;
        CountryFlag = country.FlagEmoji;
    }

    private void OnCountryChanged(object? sender, string e)
    {
        LoadPhoneCodeFromCurrentCountry();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var pObj) && pObj is Property property)
            Property = property;
        if (query.TryGetValue("room", out var rObj) && rObj is Room room)
            Room = room;
    }

    private async void OnContinue()
    {
        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(DocumentNumber))
        {
            await Shell.Current.DisplayAlert("Error", "Por favor completa todos los campos", "OK");
            return;
        }

        if (!IsValidEmail(Email))
        {
            await Shell.Current.DisplayAlert("Error", "Por favor ingresa un email válido", "OK");
            return;
        }


        var traveler = new Traveler
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Phone = FullPhoneNumber,
            DocumentType = "CC",
            DocumentNumber = DocumentNumber
        };

        var navParams = new Dictionary<string, object>
        {
            { "property", Property },
            { "room", Room },
            { "traveler", traveler }
        };
        await Shell.Current.GoToAsync("BookingSummaryPage", navParams);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

}
