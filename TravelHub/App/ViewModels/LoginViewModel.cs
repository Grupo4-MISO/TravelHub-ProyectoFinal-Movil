using System.Windows.Input;

namespace App.ViewModels;

public class LoginViewModel : BaseViewModel
{
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

    public LoginViewModel()
    {
        Title = "Iniciar Sesion";
        LoginCommand = new Command(OnLogin);
        RegisterCommand = new Command(OnRegister);
    }

    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa tu email y contrasena.", "OK");
            return;
        }

        // Mock login - navigate to account
        await Shell.Current.GoToAsync("//account");
    }

    private async void OnRegister()
    {
        await Shell.Current.DisplayAlert("Registro", "Funcion de registro proximamente.", "OK");
    }
}
