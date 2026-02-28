using System.Windows.Input;
using App.Models;
using App.Services;

namespace App.ViewModels;

public class AccountViewModel : BaseViewModel
{
    private Traveler _user = new();
    public Traveler User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    private bool _isLoggedIn = true;
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

    public AccountViewModel()
    {
        Title = "Mi Cuenta";
        User = MockDataService.GetSampleTraveler();

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
            IsLoggedIn = false;
            Email = string.Empty;
            Password = string.Empty;
        }
    }

    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa tu email y contrasena.", "OK");
            return;
        }

        User = MockDataService.GetSampleTraveler();
        IsLoggedIn = true;
    }

    private async void OnRegister()
    {
        await Shell.Current.DisplayAlert("Registro", "Funcion de registro proximamente.", "OK");
    }
}
