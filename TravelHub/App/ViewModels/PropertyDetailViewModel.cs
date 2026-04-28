using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services.Interfaces;

namespace App.ViewModels;

public class PropertyDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IPropertyDetailService _propertyDetailService;

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
                OnPropertyChanged(nameof(HasCoordinates));
            }
        }
    }

    public ObservableCollection<string> ImageUrls { get; } = [];
    public ObservableCollection<Review> Reviews { get; } = [];

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasCoordinates => Property.Latitude != 0 || Property.Longitude != 0;

    private string _reviewsErrorMessage = string.Empty;
    public string ReviewsErrorMessage
    {
        get => _reviewsErrorMessage;
        set
        {
            if (SetProperty(ref _reviewsErrorMessage, value))
            {
                OnPropertyChanged(nameof(HasReviewsError));
            }
        }
    }

    public bool HasReviewsError => !string.IsNullOrWhiteSpace(ReviewsErrorMessage);

    public ICommand ChooseRoomCommand { get; }

    public PropertyDetailViewModel(IPropertyDetailService propertyDetailService)
    {
        _propertyDetailService = propertyDetailService ?? throw new ArgumentNullException(nameof(propertyDetailService));
        Title = "Detalle";
        ChooseRoomCommand = new Command(OnChooseRoom);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var obj) && obj is Property property)
        {
            Property = property;
            Title = property.Name;
            _ = LoadPropertyDetailAsync(property);
        }
    }

    private async Task LoadPropertyDetailAsync(Property selectedProperty)
    {
        if (IsBusy)
        {
            return;
        }

        var propertyId = ResolvePropertyId(selectedProperty);
        if (string.IsNullOrWhiteSpace(propertyId))
        {
            ErrorMessage = "No se pudo identificar el alojamiento seleccionado.";
            return;
        }

        IsBusy = true;
        try
        {
            ErrorMessage = string.Empty;
            ReviewsErrorMessage = string.Empty;
            var currencyCode = selectedProperty.Country?.CurrencyCode ?? "COP";

            var detailTask = _propertyDetailService.GetPropertyDetailAsync(propertyId, currencyCode);
            var reviewsTask = _propertyDetailService.GetPropertyReviewsAsync(propertyId);
            await Task.WhenAll(detailTask, reviewsTask);

            var detailResponse = await detailTask;
            if (detailResponse.Error || detailResponse.Response == null)
            {
                ErrorMessage = await detailResponse.GetErrorMessageAsync();
                return;
            }

            var propertyFromApi = detailResponse.Response;
            if (propertyFromApi.Country != null && string.IsNullOrWhiteSpace(propertyFromApi.Country.CurrencyCode))
            {
                propertyFromApi.Country.CurrencyCode = string.IsNullOrWhiteSpace(currencyCode) ? "COP" : currencyCode;
            }

            if (string.IsNullOrWhiteSpace(propertyFromApi.ProviderId))
            {
                propertyFromApi.ProviderId = propertyId;
            }

            var reviewsResponse = await reviewsTask;
            if (reviewsResponse.Error || reviewsResponse.Response == null)
            {
                ReviewsErrorMessage = await reviewsResponse.GetErrorMessageAsync();
            }
            else
            {
                propertyFromApi.Reviews = reviewsResponse.Response;
            }

            Property = propertyFromApi;
            Title = propertyFromApi.Name;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string ResolvePropertyId(Property property)
    {
        if (!string.IsNullOrWhiteSpace(property.ProviderId))
        {
            return property.ProviderId.Trim();
        }

        return property.Id > 0 ? property.Id.ToString() : string.Empty;
    }

    private async void OnChooseRoom()
    {
        var navParams = new Dictionary<string, object> { { "property", Property } };
        await Shell.Current.GoToAsync("RoomSelectionPage", navParams);
    }
}
