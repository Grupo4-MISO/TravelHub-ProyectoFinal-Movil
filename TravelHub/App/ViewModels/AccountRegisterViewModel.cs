using App.DTOs;
using App.Services.Interfaces;

namespace App.ViewModels;

public class AccountRegisterViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IAuthService _authService;

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string _documentNumber = string.Empty;
    public string DocumentNumber
    {
        get => _documentNumber;
        set => SetProperty(ref _documentNumber, value);
    }

    private string _gender = "Other";
    public string Gender
    {
        get => _gender;
        set => SetProperty(ref _gender, value);
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

    private string _phoneNumber = string.Empty;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    private bool _acceptTerms = false;
    public bool AcceptTerms
    {
        get => _acceptTerms;
        set => SetProperty(ref _acceptTerms, value);
    }

    private string _phoneCode = string.Empty;
    public string PhoneCode
    {
        get => _phoneCode;
        set => SetProperty(ref _phoneCode, value);
    }

    private string _countryFlag = string.Empty;
    public string CountryFlag
    {
        get => _countryFlag;
        set => SetProperty(ref _countryFlag, value);
    }

    public Command RegisterCommand { get; }
    public Command GoToLoginCommand { get; }

    public AccountRegisterViewModel(INavigationService navigationService, IAppSettingsService appSettingsService, IAuthService authService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        Title = "Registro";
        LoadPhoneCode();
        RegisterCommand = new Command(async () => await Register());
        GoToLoginCommand = new Command(async () => await GoToLogin());
    }

    private void LoadPhoneCode()
    {
        var country = _appSettingsService.CurrentCountry;
        PhoneCode = country?.PhoneCode ?? string.Empty;
        CountryFlag = country?.FlagEmoji ?? string.Empty;
    }

    public string FullPhoneNumber => $"{PhoneCode} {PhoneNumber}".Trim();

    private async Task Register()
    {
        if (!await ValidateForm())
            return;

        IsBusy = true;

        try
        {
            var request = new TravelerCreateRequestDTO
            {
                traveler = new TravelerCreateDTO
                {
                    documentNumber = DocumentNumber.Trim(),
                    first_name = FirstName.Trim(),
                    last_name = LastName.Trim(),
                    email = Email.Trim(),
                    password = Password,
                    phone = FullPhoneNumber,
                    gender = Gender,
                    photo = string.Empty,
                    travelerStatus = "Pending"
                },
                address = new AddressCreateDTO()
            };

            var response = await _authService.RegisterAsync(request);

            if (response.Error)
            {
                var errorMessage = "Error al crear la cuenta. Intenta nuevamente.";
                if (response.HttpResponseMessage != null)
                {
                    var content = await response.HttpResponseMessage.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                        errorMessage = content;
                }
                await _navigationService.DisplayAlertAsync("Error", errorMessage, "OK");
                return;
            }

            await _navigationService.DisplayAlertAsync(
                "Registro exitoso",
                "Tu cuenta ha sido creada. Ahora puedes iniciar sesión.",
                "OK");

            await _navigationService.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _navigationService.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToLogin()
    {
        await _navigationService.GoToAsync("..");
    }

    private async Task<bool> ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa tu nombre", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(DocumentNumber))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa tu numero de documento", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa tu apellido", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa tu email", "OK");
            return false;
        }

        if (!IsValidEmail(Email))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa un email válido", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa tu teléfono", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await _navigationService.DisplayAlertAsync("Error", "Por favor ingresa una contraseña", "OK");
            return false;
        }

        if (Password.Length < 6)
        {
            await _navigationService.DisplayAlertAsync("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
            return false;
        }

        if (Password != ConfirmPassword)
        {
            await _navigationService.DisplayAlertAsync("Error", "Las contraseñas no coinciden", "OK");
            return false;
        }

        if (!AcceptTerms)
        {
            await _navigationService.DisplayAlertAsync("Error", "Debes aceptar los términos y condiciones", "OK");
            return false;
        }

        return true;
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
