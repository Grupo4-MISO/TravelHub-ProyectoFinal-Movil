namespace App.Services.Interfaces;

public interface IMainThreadService
{
    bool IsMainThread { get; }
    void BeginInvokeOnMainThread(Action action);
}
