using App.ViewModels;

namespace App.Views;

public partial class AccountRegisterPage : ContentPage
{
    public AccountRegisterPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetRequiredService<AccountRegisterViewModel>();
    }
}
