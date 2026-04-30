using App.Models;
using App.Providers.Interfaces;
using App.Repositories.Interfaces;
using App.Responses;
using App.Services.Interfaces;

namespace App.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IBackEndService _backEndService;
        private readonly IBackendUrlProvider _backendUrlProvider;
        private readonly ICountryRepository _countryRepository;
        private List<Country>? _cachedCountries;

        private const string CountriesEndpoint = "/api/v1/inventarios/countries";

        public CountryService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider, ICountryRepository countryRepository)
        {
            _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
            _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
            _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
            _backendUrlProvider.BaseUrlChanged += OnBackendUrlChanged;
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

                var countriesFromRepository = await _countryRepository.GetCountriesAsync();
                if (countriesFromRepository.Count > 0)
                {
                    _cachedCountries = countriesFromRepository.ToList();
                    return new HttpResponseWrapper<List<Country>>(
                        _cachedCountries,
                        false,
                        new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    );
                }

                var response = await _backEndService.GetAsync<List<Country>>(_backendUrlProvider.Build(CountriesEndpoint));

                if (!response.Error && response.Response != null && response.Response.Count > 0)
                {
                    _cachedCountries = response.Response;
                    await _countryRepository.SaveCountriesAsync(_cachedCountries);
                }

                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo países: {ex.Message}");
                return new HttpResponseWrapper<List<Country>>(
                    [],
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

        private void OnBackendUrlChanged(object? sender, string newBaseUrl)
        {
            _cachedCountries = null;
        }
    }
}
