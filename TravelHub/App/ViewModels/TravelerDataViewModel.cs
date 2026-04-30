using App.DTOs;
using App.Models;
using App.Services.Interfaces;
using App.Views;
using System.Windows.Input;

namespace App.ViewModels;

public class TravelerDataViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IUserSessionService _userSessionService;
    private readonly ITravelerProfileService _travelerProfileService;
    private readonly IAppSettingsService _appSettingsService;
    private bool _travelerLoaded;
    private bool _loginNavigationInProgress;
    private bool _suppressDirtyTracking;

    private string _imageUrl = string.Empty;
    private string _travelerId = string.Empty;
    private string _originalFirstName = string.Empty;
    private string _originalLastName = string.Empty;
    private string _originalDocumentNumber = string.Empty;
    private string _originalPhone = string.Empty;

    private AccommodationDetailDto _property = new();
    public AccommodationDetailDto Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    private AccommodationDetailRoomDto _room = new();
    public AccommodationDetailRoomDto Room
    {
        get => _room;
        set => SetProperty(ref _room, value);
    }

    private string ImageUrl
    {
        get => _imageUrl;
        set => SetProperty(ref _imageUrl, value);
    }
    public string TravelerId
    {
        get => _travelerId;
        set => SetProperty(ref _travelerId, value);
    }

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (SetProperty(ref _firstName, value))
            {
                RefreshProfileChangeState();
            }
        }
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value))
            {
                RefreshProfileChangeState();
            }
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _phoneNumber = string.Empty;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (SetProperty(ref _phoneNumber, value))
            {
                OnPropertyChanged(nameof(FullPhoneNumber));
                RefreshProfileChangeState();
            }
        }
    }

    private string _phoneCode = string.Empty;
    public string PhoneCode
    {
        get => _phoneCode;
        set
        {
            if (SetProperty(ref _phoneCode, value))
            {
                OnPropertyChanged(nameof(FullPhoneNumber));
                RefreshProfileChangeState();
            }
        }
    }

    private string _countryFlag = string.Empty;
    public string CountryFlag
    {
        get => _countryFlag;
        set => SetProperty(ref _countryFlag, value);
    }

    private string _documentNumber = string.Empty;
    public string DocumentNumber
    {
        get => _documentNumber;
        set
        {
            if (SetProperty(ref _documentNumber, value))
            {
                RefreshProfileChangeState();
            }
        }
    }

    public string FullPhoneNumber
    {
        get
        {
            var phone = (PhoneNumber ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }

            if (phone.StartsWith("+", StringComparison.Ordinal))
            {
                return phone;
            }

            return $"{PhoneCode} {phone}".Trim();
        }
    }

    public bool HasPendingTravelerChanges =>
        _travelerLoaded &&
        !string.Equals(Normalize(FirstName), Normalize(_originalFirstName), StringComparison.Ordinal) ||
        _travelerLoaded &&
        !string.Equals(Normalize(LastName), Normalize(_originalLastName), StringComparison.Ordinal) ||
        _travelerLoaded &&
        !string.Equals(Normalize(DocumentNumber), Normalize(_originalDocumentNumber), StringComparison.Ordinal) ||
        _travelerLoaded &&
        !string.Equals(Normalize(FullPhoneNumber), Normalize(_originalPhone), StringComparison.Ordinal);

    public bool ShowContinueButton => !HasPendingTravelerChanges;
    public bool ShowUpdateButton => HasPendingTravelerChanges;

    public ICommand ContinueCommand { get; }
    public ICommand UpdateTravelerCommand { get; }

    public TravelerDataViewModel(IUserSessionService userSessionService, ITravelerProfileService travelerProfileService, IAppSettingsService appSettingsService)
    {
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        _travelerProfileService = travelerProfileService ?? throw new ArgumentNullException(nameof(travelerProfileService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        Title = "Datos del Viajero";
        LoadPhoneCodeFromCurrentCountry();
        ContinueCommand = new Command(OnContinue);
        UpdateTravelerCommand = new Command(OnUpdateTravelerData);
        _appSettingsService.CountryChanged += OnCountryChanged;
    }

    private void LoadPhoneCodeFromCurrentCountry()
    {
        var country = _appSettingsService.CurrentCountry;
        PhoneCode = country?.PhoneCode ?? string.Empty;
        CountryFlag = country?.FlagEmoji ?? string.Empty;
    }

    private void OnCountryChanged(object? sender, string _)
    {
        LoadPhoneCodeFromCurrentCountry();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var pObj) && pObj is AccommodationDetailDto property)
        {
            Property = property;
            ImageUrl = property.Images?.FirstOrDefault()?.Url ?? string.Empty;
        }
        if (query.TryGetValue("room", out var rObj) && rObj is AccommodationDetailRoomDto room)
        {
            Room = room;
        }
    }

    public async Task EnsureAuthenticatedAndLoadTravelerAsync()
    {
        if (!_userSessionService.IsAuthenticated)
        {
            if (_loginNavigationInProgress)
            {
                return;
            }

            _loginNavigationInProgress = true;
            await Shell.Current.GoToAsync(nameof(AccountLoginPage), new Dictionary<string, object>
            {
                { "returnTo", nameof(TravelerDataPage) }
            });
            return;
        }

        _loginNavigationInProgress = false;
        await LoadTravelerDataAsync();
    }

    private async Task LoadTravelerDataAsync()
    {
        if (_travelerLoaded || IsBusy)
        {
            return;
        }

        var userId = _userSessionService.User.Id;
        if (string.IsNullOrWhiteSpace(userId))
        {
            await Shell.Current.DisplayAlert("Error", "No se encontró un usuario autenticado.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var response = await _travelerProfileService.GetTravelerByUserIdAsync(userId);
            if (response.Error || response.Response == null)
            {
                var message = await response.GetErrorMessageAsync();
                await Shell.Current.DisplayAlert(
                    "Error",
                    string.IsNullOrWhiteSpace(message) ? "No fue posible cargar los datos del viajero." : message,
                    "OK");
                return;
            }

            var traveler = response.Response;
            _suppressDirtyTracking = true;
            TravelerId = traveler.Id;
            FirstName = traveler.FirstName ?? string.Empty;
            LastName = traveler.LastName ?? string.Empty;
            Email = traveler.Email ?? string.Empty;
            DocumentNumber = traveler.DocumentNumber ?? string.Empty;
            PhoneNumber = NormalizePhoneForInput(traveler.Phone);
            _suppressDirtyTracking = false;

            _originalFirstName = FirstName;
            _originalLastName = LastName;
            _originalDocumentNumber = DocumentNumber;
            _originalPhone = FullPhoneNumber;
            _travelerLoaded = true;
            RefreshProfileChangeState();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnUpdateTravelerData()
    {
        if (!HasPendingTravelerChanges)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(DocumentNumber))
        {
            await Shell.Current.DisplayAlert("Error", "Completa los datos requeridos para actualizar.", "OK");
            return;
        }

        var userId = _userSessionService.User.Id;
        if (string.IsNullOrWhiteSpace(userId))
        {
            await Shell.Current.DisplayAlert("Error", "No se encontró el usuario autenticado.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(TravelerId))
        {
            await Shell.Current.DisplayAlert("Error", "No se encontró el perfil del cliente.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var payload = new TravelerUpdateRequest
            {
                DocumentNumber = DocumentNumber.Trim(),
                FirstName = FirstName.Trim(),
                Gender = "Female",
                LastName = LastName.Trim(),
                Phone = FullPhoneNumber
            };

            var response = await _travelerProfileService.UpdateTravelerAsync(TravelerId, payload);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await Shell.Current.DisplayAlert(
                    "Error",
                    string.IsNullOrWhiteSpace(message) ? "No fue posible actualizar los datos." : message,
                    "OK");
                return;
            }

            _originalFirstName = FirstName;
            _originalLastName = LastName;
            _originalDocumentNumber = DocumentNumber;
            _originalPhone = FullPhoneNumber;
            RefreshProfileChangeState();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnContinue()
    {
        if (HasPendingTravelerChanges)
        {
            return;
        }

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
            first_name = FirstName,
            last_name = LastName,
            email = Email,
            phone = FullPhoneNumber,
            documentNumber = DocumentNumber
        };

        var navParams = new Dictionary<string, object>
        {
            { "property", Property },
            { "room", Room },
            { "traveler", traveler }
        };
        await Shell.Current.GoToAsync("BookingSummaryPage", navParams);
    }

    private void RefreshProfileChangeState()
    {
        if (_suppressDirtyTracking)
        {
            return;
        }

        OnPropertyChanged(nameof(HasPendingTravelerChanges));
        OnPropertyChanged(nameof(ShowContinueButton));
        OnPropertyChanged(nameof(ShowUpdateButton));
    }

    private string NormalizePhoneForInput(string? rawPhone)
    {
        var phone = (rawPhone ?? string.Empty).Trim();
        var code = (PhoneCode ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(code) &&
            phone.StartsWith(code, StringComparison.OrdinalIgnoreCase))
        {
            return phone[code.Length..].TrimStart();
        }

        return phone;
    }

    private static string Normalize(string? value) => (value ?? string.Empty).Trim();

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
