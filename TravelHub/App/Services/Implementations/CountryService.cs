using App.Models;
using App.Responses;
using App.Services.Interfaces;

namespace App.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IBackEndService _backEndService;
        private readonly IBackendUrlProvider _backendUrlProvider;
        private List<Country>? _cachedCountries;
        private readonly Dictionary<string, List<string>> _popularCitiesByCountryCache = new(StringComparer.OrdinalIgnoreCase);

        private const string CountriesEndpoint = "/api/v1/inventarios/countries";
        private const string PopularCitiesEndpointTemplate = "/api/v1/inventarios/countries/{0}/popular-cities";

        public CountryService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider)
        {
            _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
            _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
        }

        public async Task<HttpResponseWrapper<List<Country>>> GetCountriesAsync()
        {
            try
            {
                // Si tenemos datos en caché, devolverlos inmediatamente
                if (_cachedCountries != null && _cachedCountries.Count > 0)
                {
                    return new HttpResponseWrapper<List<Country>>(
                        _cachedCountries,
                        false,
                        new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    );
                }

                // Obtener países desde el backend
                var response = await _backEndService.GetAsync<List<Country>>(_backendUrlProvider.Build(CountriesEndpoint));

                // Si la respuesta es exitosa, guardar en caché
                if (!response.Error && response.Response != null)
                {
                    _cachedCountries = response.Response;
                }

                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo países: {ex.Message}");
                return new HttpResponseWrapper<List<Country>>(
                    default,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }

        public Country? GetCountryByCode(string code)
        {
            if (_cachedCountries == null || _cachedCountries.Count == 0)
                return null;

            return _cachedCountries.FirstOrDefault(c => 
                c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public Country? GetCountryById(string id)
        {
            if (_cachedCountries == null || _cachedCountries.Count == 0)
                return null;

            return _cachedCountries.FirstOrDefault(c => c.Id == id);
        }

        public async Task<HttpResponseWrapper<List<string>>> GetPopularCitiesByCountryAsync(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return new HttpResponseWrapper<List<string>>(
                    default,
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

            var endpoint = _backendUrlProvider.Build(string.Format(PopularCitiesEndpointTemplate, normalizedCountryCode));
            var response = await _backEndService.GetAsync<List<string>>(endpoint);

            if (!response.Error && response.Response != null)
            {
                _popularCitiesByCountryCache[normalizedCountryCode] = response.Response;
            }

            return response;
        }
    }
}
