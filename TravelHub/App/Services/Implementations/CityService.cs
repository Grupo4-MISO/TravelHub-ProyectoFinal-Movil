using App.Models;
using App.Providers.Interfaces;
using App.Repositories.Implementations;
using App.Repositories.Interfaces;
using App.Responses;
using App.Services.Interfaces;

namespace App.Services.Implementations
{
    public class CityService : ICityService
    {
        private readonly IBackEndService _backEndService;
        private readonly IBackendUrlProvider _backendUrlProvider;
        private readonly ICityRepository _cityRepository;
        private List<City>? _cachedCities;
        private readonly Dictionary<string, List<string>> _popularCitiesByCountryCache = new(StringComparer.OrdinalIgnoreCase);
        private const string PopularCitiesEndpointTemplate = "/api/v1/inventarios/countries/{0}/popular-cities";

        public CityService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider, ICityRepository cityRepository)
        {
            _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
            _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _backendUrlProvider.BaseUrlChanged += OnBackendUrlChanged;
        }

        public async Task<HttpResponseWrapper<List<string>>> GetPopularCitiesByCountryAsync(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return new HttpResponseWrapper<List<string>>(
                    [],
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));
            }

            var normalizedCountryCode = countryCode.Trim().ToUpperInvariant();

            if (_popularCitiesByCountryCache.TryGetValue(normalizedCountryCode, out var cachedCities) && cachedCities.Count > 0)
            {
                return new HttpResponseWrapper<List<string>>(
                    cachedCities,
                    false,
                    new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }

            var cities = await _cityRepository.GetCitiesAsync(normalizedCountryCode);
            if (cities != null && cities.Count > 0)
            {
                var cityNames = cities.Select(c => c.Name).ToList();
                _popularCitiesByCountryCache[normalizedCountryCode] = cityNames;
                return new HttpResponseWrapper<List<string>>(
                    cityNames,
                    false,
                    new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }



            var endpoint = _backendUrlProvider.Build(string.Format(PopularCitiesEndpointTemplate, normalizedCountryCode));
            var response = await _backEndService.GetAsync<List<string>>(endpoint);

            if (!response.Error && response.Response != null)
            {
                _popularCitiesByCountryCache[normalizedCountryCode] = response.Response;
                foreach (var cityName in response.Response)
                {
                    if (_cachedCities == null)
                    {
                        _cachedCities = new List<City>();
                    }
                    if (!_cachedCities.Any(c => string.Equals(c.Name, cityName, StringComparison.OrdinalIgnoreCase)))
                    {
                        _cachedCities.Add(new City { Name = cityName });
                    }
                    await _cityRepository.SaveCitiesAsync(_cachedCities);
                }
            }

            return response;
        }

        private void OnBackendUrlChanged(object? sender, string newBaseUrl)
        {
            _popularCitiesByCountryCache.Clear();
        }
    }
}
