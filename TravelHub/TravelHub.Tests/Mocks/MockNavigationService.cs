using App.Services.Interfaces;

namespace TravelHub.Tests.Mocks;

public class MockNavigationService : INavigationService
{
    public string? LastAlertTitle { get; private set; }
    public string? LastAlertMessage { get; private set; }
    public int DisplayAlertCallCount { get; private set; }

    public string? LastNavigationUri { get; private set; }
    public int GoToAsyncCallCount { get; private set; }

    public Task DisplayAlert(string title, string message, string cancel)
    {
        LastAlertTitle = title;
        LastAlertMessage = message;
        DisplayAlertCallCount++;
        return Task.CompletedTask;
    }

    public Task GoToAsync(string uri)
    {
        LastNavigationUri = uri;
        GoToAsyncCallCount++;
        return Task.CompletedTask;
    }
}