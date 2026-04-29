using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class AccountLoginPage : ContentPage
{
    public AccountLoginPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetRequiredService<AccountLoginViewModel>();
    }
}
