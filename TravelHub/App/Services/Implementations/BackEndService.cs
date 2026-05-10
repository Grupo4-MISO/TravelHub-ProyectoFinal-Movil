using Microsoft.Maui.Networking;
using App.Responses;
using App.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace App.Services.Implementations
{
    public class BackEndService : IBackEndService
    {
        public event Action? OnUnauthorizedResponse;

        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;

        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public BackEndService()
        {
            _httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true; // ⚠️ solo pruebas
                }
            };

            _httpClient = new HttpClient(_httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(60)
            };

            _httpClient.DefaultRequestHeaders.Authorization = null!;
        }

        public async Task SetBaseAddress(string url)
        {
            _httpClient.BaseAddress = new Uri(url);
        }

        public async Task SetAuthorization(string? token)
        {
            if (string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = null!;
            else
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public Response CheckConnection()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                };
            }

            return new Response
            {
                IsSuccess = false,
                Message = "Compruebe su conexión a internet.",
            };
        }

        public async Task<HttpResponseWrapper<object>> GetAsync(string url)
        {
            try
            {
                var responseHTTP = await _httpClient.GetAsync(url);

                if (responseHTTP.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                return new HttpResponseWrapper<object>(
                    null!,
                    !responseHTTP.IsSuccessStatusCode,
                    responseHTTP
                );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Timeout o cancelado: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }

        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
        {
            try
            {
                var responseHttp = await _httpClient.GetAsync(url);

                if (responseHttp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                if (responseHttp.IsSuccessStatusCode)
                {
                    var response = await UnserializeAnswer<T>(responseHttp);
                    return new HttpResponseWrapper<T>(response, false, responseHttp);
                }

                return new HttpResponseWrapper<T>(default!, true, responseHttp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<T>(default!, true, new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
            }
        }

        public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
        {
            try
            {
                //var messageJSON = JsonSerializer.Serialize(model);
                var messageJSON = JsonConvert.SerializeObject(model);
                var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");

                var responseHttp = await _httpClient.PostAsync(url, messageContent);

                if (responseHttp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                return new HttpResponseWrapper<object>(
                    null!,
                    !responseHttp.IsSuccessStatusCode,
                    responseHttp
                );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Timeout o cancelación: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }

        public async Task<HttpResponseWrapper<TResponse>> PostAsync<T, TResponse>(string url, T model)
        {
            try
            {
                //var messageJSON = JsonSerializer.Serialize(model);
                var messageJSON = JsonConvert.SerializeObject(model);
                var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");

                var responseHttp = await _httpClient.PostAsync(url, messageContent);

                if (responseHttp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                if (responseHttp.IsSuccessStatusCode)
                {
                    var response = await UnserializeAnswer<TResponse>(responseHttp);
                    return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
                }

                return new HttpResponseWrapper<TResponse>(default!, true, responseHttp);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Timeout o cancelación: " + ex.Message);

                return new HttpResponseWrapper<TResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);

                return new HttpResponseWrapper<TResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<TResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }

        private async Task<T> UnserializeAnswer<T>(HttpResponseMessage responseHttp)
        {
            var response = await responseHttp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(response)!;
        }

        public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
        {
            try
            {
                var responseHttp = await _httpClient.DeleteAsync(url);

                if (responseHttp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                return new HttpResponseWrapper<object>(
                    null!,
                    !responseHttp.IsSuccessStatusCode,
                    responseHttp
                );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Timeout o cancelación: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }

        public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
        {
            try
            {
                //var messageJSON = JsonSerializer.Serialize(model);
                var messageJSON = JsonConvert.SerializeObject(model);
                var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");

                var responseHttp = await _httpClient.PutAsync(url, messageContent);

                if (responseHttp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                return new HttpResponseWrapper<object>(
                    null!,
                    !responseHttp.IsSuccessStatusCode,
                    responseHttp
                );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Timeout o cancelación: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<object>(
                    null!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }

        public async Task<HttpResponseWrapper<TResponse>> PutAsync<T, TResponse>(string url, T model)
        {
            try
            {
                //var messageJSON = JsonSerializer.Serialize(model);
                var messageJSON = JsonConvert.SerializeObject(model);
                var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");

                var responseHttp = await _httpClient.PutAsync(url, messageContent);

                if (responseHttp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizedResponse?.Invoke();
                }

                if (responseHttp.IsSuccessStatusCode)
                {
                    var response = await UnserializeAnswer<TResponse>(responseHttp);
                    return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
                }

                return new HttpResponseWrapper<TResponse>(default!, true, responseHttp);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Timeout o cancelación: " + ex.Message);

                return new HttpResponseWrapper<TResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);

                return new HttpResponseWrapper<TResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return new HttpResponseWrapper<TResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                );
            }
        }
    }
}
