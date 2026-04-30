using App.Services.Interfaces;
using Microsoft.Maui.ApplicationModel;

namespace App.Services.Implementations;

public class MainThreadService : IMainThreadService
{
    public bool IsMainThread => MainThread.IsMainThread;

    public void BeginInvokeOnMainThread(Action action)
    {
        MainThread.BeginInvokeOnMainThread(action);
    }
}
