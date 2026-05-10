namespace App.Services.Interfaces;

public interface INavigationService
{
    Task DisplayAlertAsync(string title, string message, string cancel);
    Task GoToAsync(string uri);
}
