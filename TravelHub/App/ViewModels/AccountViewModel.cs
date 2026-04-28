using System.Windows.Input;
using App.Models;
using App.Services.Interfaces;
using App.Views;

namespace App.ViewModels;

public class AccountViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IUserSessionService _userSessionService;

    private Traveler _user = new();
    public Traveler User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    private bool _isLoggedIn;
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            if (SetProperty(ref _isLoggedIn, value))
                OnPropertyChanged(nameof(IsNotLoggedIn));
        }
    }

    public bool IsNotLoggedIn => !IsLoggedIn;

    // Login fields
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

    public ICommand EditProfileCommand { get; }
    public ICommand ViewHistoryCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    public AccountViewModel(IAuthService authService, IUserSessionService userSessionService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        Title = "Mi Cuenta";
        LoadCurrentSession();

        EditProfileCommand = new Command(OnEditProfile);
        ViewHistoryCommand = new Command(OnViewHistory);
        LogoutCommand = new Command(OnLogout);
        LoginCommand = new Command(OnLogin);
        RegisterCommand = new Command(OnRegister);
    }

    private async void OnEditProfile()
    {
        await Shell.Current.DisplayAlert("Editar Perfil", "Funcion disponible proximamente.", "OK");
    }

    private async void OnViewHistory()
    {
        await Shell.Current.DisplayAlert("Historial", "Funcion disponible proximamente.", "OK");
    }

    private async void OnLogout()
    {
        bool confirm = await Shell.Current.DisplayAlert("Cerrar Sesion", "Deseas cerrar tu sesion?", "Si", "No");
        if (confirm)
        {
            _userSessionService.ClearSession();
            IsLoggedIn = false;
            Email = string.Empty;
            Password = string.Empty;
            User = new Traveler();
        }
    }

    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa tu email y contrasena.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var response = await _authService.LoginAsync(new AuthLoginRequest
            {
                Email = Email.Trim(),
                Password = Password
            });

            if (response.Error || response.Response == null || string.IsNullOrWhiteSpace(response.Response.Token))
            {
                var errorMessage = await response.GetErrorMessageAsync();
                await Shell.Current.DisplayAlert(
                    "Error",
                    string.IsNullOrWhiteSpace(errorMessage) ? "No fue posible iniciar sesión." : errorMessage,
                    "OK");
                return;
            }

            _userSessionService.SetSession(response.Response);
            Password = string.Empty;
            LoadCurrentSession();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnRegister()
    {
        await Shell.Current.GoToAsync(nameof(AccountRegisterPage));
    }

    private void LoadCurrentSession()
    {
        IsLoggedIn = _userSessionService.IsAuthenticated;
        if (!IsLoggedIn)
        {
            User = new Traveler();
            return;
        }

        var sessionUser = _userSessionService.User;
        User = new Traveler
        {
            FirstName = sessionUser.Username,
            LastName = string.Empty,
            Email = Email,
            Phone = string.Empty,
            DocumentType = sessionUser.Role,
            DocumentNumber = sessionUser.Id
        };
    }
}
