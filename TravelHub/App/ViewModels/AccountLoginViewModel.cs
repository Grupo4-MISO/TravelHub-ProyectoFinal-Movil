using App.Views;
using App.Models;
using App.Services.Interfaces;
using System.Windows.Input;

namespace App.ViewModels;

public class AccountLoginViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAuthService _authService;
    private readonly IUserSessionService _userSessionService;
    private readonly INavigationService _navigationService;
    private string _returnTo = string.Empty;

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    public AccountLoginViewModel(IAuthService authService, IUserSessionService userSessionService, INavigationService navigationService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        Title = "Iniciar Sesion";
        LoginCommand = new Command(OnLogin);
        RegisterCommand = new Command(OnRegister);
    }

    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await _navigationService.DisplayAlertAsync("Error", "Ingresa tu email y contrasena.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var request = new AuthLoginRequest
            {
                Email = Email.Trim(),
                Password = Password
            };

            var response = await _authService.LoginAsync(request);
            if (response.Error || response.Response == null || string.IsNullOrWhiteSpace(response.Response.Token))
            {
                var errorMessage = await response.GetErrorMessageAsync();
                await _navigationService.DisplayAlertAsync(
                    "Error",
                    string.IsNullOrWhiteSpace(errorMessage) ? "No fue posible iniciar sesion." : "Usuario o Contraseña no son correctos",
                    "OK");
                return;
            }

            await _userSessionService.SetSession(response.Response);
            Password = string.Empty;
            if (string.Equals(_returnTo, nameof(TravelerDataPage), StringComparison.Ordinal))
            {
                _returnTo = string.Empty;
                await _navigationService.GoToAsync("..");
                return;
            }

            await _navigationService.GoToAsync("//account");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnRegister()
    {
        await _navigationService.GoToAsync(nameof(AccountRegisterPage));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("returnTo", out var returnToObj) && returnToObj is string returnToValue)
        {
            _returnTo = returnToValue;
        }
    }
}
