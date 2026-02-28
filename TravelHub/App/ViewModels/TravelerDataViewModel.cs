using System.Windows.Input;
using App.Models;

namespace App.ViewModels;

public class TravelerDataViewModel : BaseViewModel, IQueryAttributable
{
    private Property _property = new();
    public Property Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    private Room _room = new();
    public Room Room
    {
        get => _room;
        set => SetProperty(ref _room, value);
    }

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    private string _documentNumber = string.Empty;
    public string DocumentNumber
    {
        get => _documentNumber;
        set => SetProperty(ref _documentNumber, value);
    }

    public ICommand ContinueCommand { get; }

    public TravelerDataViewModel()
    {
        Title = "Datos del Viajero";
        ContinueCommand = new Command(OnContinue);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var pObj) && pObj is Property property)
            Property = property;
        if (query.TryGetValue("room", out var rObj) && rObj is Room room)
            Room = room;
    }

    private async void OnContinue()
    {
        var traveler = new Traveler
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Phone = Phone,
            DocumentType = "INE",
            DocumentNumber = DocumentNumber
        };

        var navParams = new Dictionary<string, object>
        {
            { "property", Property },
            { "room", Room },
            { "traveler", traveler }
        };
        await Shell.Current.GoToAsync("BookingSummaryPage", navParams);
    }
}
