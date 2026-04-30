using App.DTOs;
using App.Models;
using App.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

public class PropertyDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IPropertyDetailService _propertyDetailService;

    private AccommodationDetailDto _property = new();
    public AccommodationDetailDto Property
    {
        get => _property;
        set
        {
            if (SetProperty(ref _property, value))
            {
                ImageUrls.Clear();
                foreach (var img in value.Images)
                    ImageUrls.Add(img.Url);
                OnPropertyChanged(nameof(HasCoordinates));
            }
        }
    }

    public ObservableCollection<string> ImageUrls { get; } = new ObservableCollection<string>();
    public ObservableCollection<AccommodationReviewDto> Reviews { get; } = new ObservableCollection<AccommodationReviewDto>();

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

    private decimal _priceforNight = 0;
    public decimal PriceforNight
    {
        get => _priceforNight;
        set => SetProperty(ref _priceforNight, value);
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
        if (query.TryGetValue("property", out var obj) && obj is SearchAccommodationDto property)
        {
            Title = property.Name;
            PriceforNight = property.Price;
            _ = LoadPropertyDetailAsync(property);
        }
    }

    private async Task LoadPropertyDetailAsync(SearchAccommodationDto property)
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            ErrorMessage = string.Empty;
            ReviewsErrorMessage = string.Empty;
            var detailTask =  _propertyDetailService.GetPropertyDetailAsync(property.PropertyId, property.CurrencyCode);
            var reviewsTask = _propertyDetailService.GetPropertyReviewsAsync(property.PropertyId);
            await Task.WhenAll(detailTask, reviewsTask);

            var detailResponse = await detailTask;
            if (detailResponse.Error || detailResponse.Response == null)
            {
                ErrorMessage = await detailResponse.GetErrorMessageAsync();
                return;
            }

            var propertyFromApi = detailResponse.Response;

            if (propertyFromApi == null)
            {
                return;
            }

            var reviewsResponse = await reviewsTask;
            if (reviewsResponse.Error || reviewsResponse.Response == null)
            {
                ReviewsErrorMessage = await reviewsResponse.GetErrorMessageAsync();
            }
            else
            {
                var reviews = reviewsResponse.Response;
                foreach (var review in reviews)
                {
                    Reviews.Add(review);
                }
            }

            Property = propertyFromApi;
            Title = propertyFromApi.Name;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnChooseRoom()
    {
        var navParams = new Dictionary<string, object> { { "property", Property } };
        await Shell.Current.GoToAsync("RoomSelectionPage", navParams);
    }
}
