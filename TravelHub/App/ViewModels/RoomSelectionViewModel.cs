using App.DTOs;
using App.Models;
using App.Services.Interfaces;
using App.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

public class RoomSelectionViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IUserSessionService _userSessionService;

    private AccommodationDetailDto _property = new();
    public AccommodationDetailDto Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    public ObservableCollection<AccommodationDetailRoomDto> Rooms { get; } = [];

    private AccommodationDetailRoomDto? _selectedRoom;
    public AccommodationDetailRoomDto? SelectedRoom
    {
        get => _selectedRoom;
        set
        {
            if (SetProperty(ref _selectedRoom, value))
                OnPropertyChanged(nameof(IsRoomSelected));
        }
    }

    public bool IsRoomSelected => SelectedRoom != null;

    public ICommand SelectRoomCommand { get; }
    public ICommand BookCommand { get; }

    public RoomSelectionViewModel(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        Title = "Elegir Habitacion";
        SelectRoomCommand = new Command<AccommodationDetailRoomDto>(OnSelectRoom);
        BookCommand = new Command(OnBook);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var obj) && obj is AccommodationDetailDto property)
        {
            Property = property;
            Rooms.Clear();
            foreach (var room in property.Rooms)
                Rooms.Add(room);
        }
    }

    private void OnSelectRoom(AccommodationDetailRoomDto? room)
    {
        SelectedRoom = room;
    }

    private async void OnBook()
    {
        if (SelectedRoom == null) return;

        await _userSessionService.ValidateSessionAsync();

        if (!_userSessionService.IsAuthenticated)
        {
            await Shell.Current.GoToAsync(nameof(AccountLoginPage), new Dictionary<string, object>
            {
                { "returnTo", nameof(TravelerDataPage) }
            });
            return;
        }

        var navParams = new Dictionary<string, object>
        {
            { "property", Property },
            { "room", SelectedRoom }
        };
        await Shell.Current.GoToAsync("TravelerDataPage", navParams);
    }
}
