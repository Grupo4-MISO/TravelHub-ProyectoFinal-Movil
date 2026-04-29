using App.ViewModels;

using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class AccountRegisterPage : ContentPage
{
    public AccountRegisterPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetRequiredService<AccountRegisterViewModel>();
    }
}
