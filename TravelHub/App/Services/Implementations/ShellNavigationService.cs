using App.Services.Interfaces;

namespace App.Services.Implementations;

public class ShellNavigationService : INavigationService
{
    public Task DisplayAlert(string title, string message, string cancel)
    {
        return Shell.Current.DisplayAlert(title, message, cancel);
    }

    public Task GoToAsync(string uri)
    {
        return Shell.Current.GoToAsync(uri);
    }
}
