using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;

namespace App.ViewModels;

public class RoomSelectionViewModel : BaseViewModel, IQueryAttributable
{
    private Property _property = new();
    public Property Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    public ObservableCollection<Room> Rooms { get; } = [];

    private Room? _selectedRoom;
    public Room? SelectedRoom
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

    public RoomSelectionViewModel()
    {
        Title = "Elegir Habitacion";
        SelectRoomCommand = new Command<Room>(OnSelectRoom);
        BookCommand = new Command(OnBook);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var obj) && obj is Property property)
        {
            Property = property;
            Rooms.Clear();
            foreach (var room in property.Rooms)
                Rooms.Add(room);
        }
    }

    private void OnSelectRoom(Room? room)
    {
        SelectedRoom = room;
    }

    private async void OnBook()
    {
        if (SelectedRoom == null) return;
        var navParams = new Dictionary<string, object>
        {
            { "property", Property },
            { "room", SelectedRoom }
        };
        await Shell.Current.GoToAsync("TravelerDataPage", navParams);
    }
}
