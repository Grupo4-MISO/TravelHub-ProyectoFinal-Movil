using App.Models;
using App.Services.Interfaces;
using App.ViewModels;
using Moq;
using Xunit;

namespace TravelHub.Tests.ViewModels;

public class AccountRegisterViewModelTests
{
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<IAppSettingsService> _appSettingsServiceMock;
    private readonly AccountRegisterViewModel _viewModel;

    public AccountRegisterViewModelTests()
    {
        _navigationServiceMock = new Mock<INavigationService>();
        _appSettingsServiceMock = new Mock<IAppSettingsService>();

        _appSettingsServiceMock.Setup(s => s.CurrentCountry)
            .Returns(new Country
            {
                Code = "CO",
                Name = "Colombia",
                PhoneCode = "+57",
                FlagEmoji = "🇨🇴"
            });

        _viewModel = new AccountRegisterViewModel(_navigationServiceMock.Object, _appSettingsServiceMock.Object);
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
        await Task.Delay(500);

        _navigationServiceMock.Verify(n => n.DisplayAlert("Error",
            It.Is<string>(msg => msg.ToLower().Contains("nombre")),
            "OK"), Times.Once);
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
        await Task.Delay(500);

        _navigationServiceMock.Verify(n => n.DisplayAlert("Error",
            It.Is<string>(msg => msg.ToLower().Contains("email válido")),
            "OK"), Times.Once);
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
        await Task.Delay(500);

        _navigationServiceMock.Verify(n => n.DisplayAlert("Error",
            It.Is<string>(msg => msg.ToLower().Contains("no coinciden")),
            "OK"), Times.Once);
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
        await Task.Delay(2500);

        _navigationServiceMock.Verify(n => n.DisplayAlert("Registro exitoso",
            It.IsAny<string>(), "OK"), Times.Once);
        _navigationServiceMock.Verify(n => n.GoToAsync(".."), Times.Once);
    }

    [Fact]
    public void FullPhoneNumber_ReturnsFormattedNumber()
    {
        _viewModel.PhoneCode = "+57";
        _viewModel.PhoneNumber = "3001234567";

        Assert.Equal("+57 3001234567", _viewModel.FullPhoneNumber);
    }

    [Fact]
    public void FullPhoneNumber_EmptyPhoneNumber_ReturnsPhoneCodeOnly()
    {
        _viewModel.PhoneCode = "+57";
        _viewModel.PhoneNumber = string.Empty;

        Assert.Equal("+57", _viewModel.FullPhoneNumber.Trim());
    }

    [Fact]
    public void Constructor_Throws_WhenNavigationServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountRegisterViewModel(null!, _appSettingsServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenAppSettingsServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountRegisterViewModel(_navigationServiceMock.Object, null!));
    }
}
