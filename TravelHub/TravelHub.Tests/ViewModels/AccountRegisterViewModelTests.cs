using App.ViewModels;
using TravelHub.Tests.Mocks;
using Xunit;

namespace TravelHub.Tests.ViewModels;

public class AccountRegisterViewModelTests
{
    private readonly MockNavigationService _mockNavigationService;
    private readonly MockAppSettingsService _mockAppSettingsService;
    private readonly AccountRegisterViewModel _viewModel;

    public AccountRegisterViewModelTests()
    {
        _mockNavigationService = new MockNavigationService();
        _mockAppSettingsService = new MockAppSettingsService();
        _viewModel = new AccountRegisterViewModel(_mockNavigationService, _mockAppSettingsService);
    }

    [Fact]
    public void Constructor_SetsTitleAndLoadsCountryData()
    {
        Assert.Equal("Registro", _viewModel.Title);
        Assert.Equal("+57", _viewModel.PhoneCode);
        Assert.Equal("🇨🇴", _viewModel.CountryFlag);
    }

    [Fact]
    public async Task RegisterCommand_EmptyFirstName_ShowsError()
    {
        _viewModel.LastName = "Perez";
        _viewModel.Email = "test@email.com";
        _viewModel.PhoneNumber = "3001234567";
        _viewModel.Password = "password123";
        _viewModel.ConfirmPassword = "password123";
        _viewModel.AcceptTerms = true;

        _viewModel.RegisterCommand.Execute(null);
        await Task.Delay(100);

        Assert.Equal("Error", _mockNavigationService.LastAlertTitle);
        Assert.Contains("nombre", _mockNavigationService.LastAlertMessage?.ToLower());
    }

    [Fact]
    public async Task RegisterCommand_InvalidEmail_ShowsError()
    {
        _viewModel.FirstName = "Juan";
        _viewModel.LastName = "Perez";
        _viewModel.Email = "correo-invalido";
        _viewModel.PhoneNumber = "3001234567";
        _viewModel.Password = "password123";
        _viewModel.ConfirmPassword = "password123";
        _viewModel.AcceptTerms = true;

        _viewModel.RegisterCommand.Execute(null);
        await Task.Delay(100);

        Assert.Equal("Error", _mockNavigationService.LastAlertTitle);
        Assert.Contains("email válido", _mockNavigationService.LastAlertMessage);
    }

    [Fact]
    public async Task RegisterCommand_PasswordMismatch_ShowsError()
    {
        _viewModel.FirstName = "Juan";
        _viewModel.LastName = "Perez";
        _viewModel.Email = "test@email.com";
        _viewModel.PhoneNumber = "3001234567";
        _viewModel.Password = "password123";
        _viewModel.ConfirmPassword = "password321";
        _viewModel.AcceptTerms = true;

        _viewModel.RegisterCommand.Execute(null);
        await Task.Delay(100);

        Assert.Equal("Error", _mockNavigationService.LastAlertTitle);
        Assert.Contains("no coinciden", _mockNavigationService.LastAlertMessage?.ToLower());
    }

    [Fact]
    public async Task RegisterCommand_ValidForm_ShowsSuccessAndNavigatesToLogin()
    {
        _viewModel.FirstName = "Juan";
        _viewModel.LastName = "Perez";
        _viewModel.Email = "test@email.com";
        _viewModel.PhoneNumber = "3001234567";
        _viewModel.Password = "password123";
        _viewModel.ConfirmPassword = "password123";
        _viewModel.AcceptTerms = true;

        _viewModel.RegisterCommand.Execute(null);
        await Task.Delay(1700);

        Assert.Equal("Registro exitoso", _mockNavigationService.LastAlertTitle);
        Assert.Equal("..", _mockNavigationService.LastNavigationUri);
    }
}
