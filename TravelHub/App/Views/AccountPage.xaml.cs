using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetRequiredService<AccountViewModel>();
    }
}
