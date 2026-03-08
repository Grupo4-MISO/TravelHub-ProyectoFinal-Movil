using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;

namespace App.ViewModels;

public class PropertyDetailViewModel : BaseViewModel, IQueryAttributable
{
    private Property _property = new();
    public Property Property
    {
        get => _property;
        set
        {
            if (SetProperty(ref _property, value))
            {
                ImageUrls.Clear();
                foreach (var url in value.ImageUrls)
                    ImageUrls.Add(url);
                Reviews.Clear();
                foreach (var r in value.Reviews)
                    Reviews.Add(r);
            }
        }
    }

    public ObservableCollection<string> ImageUrls { get; } = [];
    public ObservableCollection<Review> Reviews { get; } = [];

    public ICommand ChooseRoomCommand { get; }

    public PropertyDetailViewModel()
    {
        Title = "Detalle";
        ChooseRoomCommand = new Command(OnChooseRoom);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var obj) && obj is Property property)
        {
            Property = property;
            Title = property.Name;
        }
    }

    private async void OnChooseRoom()
    {
        var navParams = new Dictionary<string, object> { { "property", Property } };
        await Shell.Current.GoToAsync("RoomSelectionPage", navParams);
    }
}
