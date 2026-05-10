using App.DTOs;
using App.Responses;
using App.Services.Interfaces;
using App.Providers.Interfaces;

namespace App.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly IBackEndService _backEndService;
    private readonly IBackendUrlProvider _backendUrlProvider;

    public BookingService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider)
    {
        _backEndService = backEndService;
        _backendUrlProvider = backendUrlProvider;
    }

    public async Task<HttpResponseWrapper<List<BookingHoldResponseDto>>> GetUserBookingsAsync(string userId)
    {
        var url = _backendUrlProvider.Build($"/api/v1/reservas/usuario/{userId}");
        return await _backEndService.GetAsync<List<BookingHoldResponseDto>>(url);
    }

    public async Task<HttpResponseWrapper<BookingResponseDto>> GetBookingByReservationIdAsync(string bookingId)
    {
        var url = _backendUrlProvider.Build($"/api/v1/reservas/{bookingId}");
        return await _backEndService.GetAsync<BookingResponseDto>(url);
    }
    public async Task<HttpResponseWrapper<object>> CheckInBookingByReservationIdAsync(string bookingId)
    {
        var url = _backendUrlProvider.Build($"/api/v1/reservas/completar/{bookingId}");
        return await _backEndService.PostAsync(url, true);
    }
    public async Task<HttpResponseWrapper<object>> RevokeBookingByReservationIdAsync(string bookingId)
    {
        var url = _backendUrlProvider.Build($"/api/v1/reservas/revocar/{bookingId}");
        return await _backEndService.PostAsync(url, true);
    }

    public async Task<HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>> GetHotelsByRoomIdsAsync(List<string> roomIds)
    {
        var url = _backendUrlProvider.Build("/api/v1/inventarios/hoteles");
        var payload = new { habitaciones_ids = roomIds };
        return await _backEndService.PostAsync<object, Dictionary<string, HotelInventoryDto>>(url, payload);
    }

    public async Task<HttpResponseWrapper<object>> CreateReservationHoldAsync(ReservationHoldRequestDto payload)
    {
        var url = _backendUrlProvider.Build("/api/v1/reservas/hold");
        return await _backEndService.PostAsync(url, payload);
    }

    public async Task<HttpResponseWrapper<CreateReservationRequest>> CreateReservationAsync(ReservationHoldRequestDto payload)
    {
        var url = _backendUrlProvider.Build("/api/v1/reservas/crear");
        return await _backEndService.PostAsync<ReservationHoldRequestDto, CreateReservationRequest>(url, payload);
    }

    public async Task<HttpResponseWrapper<List<PaymentProviderDto>>> GetPaymentProvidersAsync()
    {
        var url = _backendUrlProvider.Build("/api/v1/Transactions/providers");
        return await _backEndService.GetAsync<List<PaymentProviderDto>>(url);
    }

    public async Task<HttpResponseWrapper<PaymentResponseDTO>> CreatePaymentAsync(PaymentRequestDTO payload)
    {
        var url = _backendUrlProvider.Build("/api/v1/Transactions/payments");
        return await _backEndService.PostAsync<PaymentRequestDTO, PaymentResponseDTO>(url, payload);
    }

    public async Task<HttpResponseWrapper<List<PaymentReservationDTO>>> GetPaymentsByReservationAsync(string reservaId)
    {
        var url = _backendUrlProvider.Build($"/api/v1/Transactions/payments/reserva/{reservaId}");
        return await _backEndService.GetAsync<List<PaymentReservationDTO>>(url);
    }
}
