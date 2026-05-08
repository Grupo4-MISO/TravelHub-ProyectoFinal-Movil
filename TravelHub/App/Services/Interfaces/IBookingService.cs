using App.DTOs;
using App.Responses;

namespace App.Services.Interfaces;

public interface IBookingService
{
    Task<HttpResponseWrapper<List<BookingHoldResponseDto>>> GetUserBookingsAsync(string userId);
    Task<HttpResponseWrapper<BookingResponseDto>> GetBookingByReservationIdAsync(string bookingId);
    Task<HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>> GetHotelsByRoomIdsAsync(List<string> roomIds);
    Task<HttpResponseWrapper<object>> CreateReservationHoldAsync(ReservationHoldRequestDto payload);
    Task<HttpResponseWrapper<CreateReservationRequest>> CreateReservationAsync(ReservationHoldRequestDto payload);
    Task<HttpResponseWrapper<List<PaymentProviderDto>>> GetPaymentProvidersAsync();
    Task<HttpResponseWrapper<PaymentResponseDTO>> CreatePaymentAsync(PaymentRequestDTO payload);
}
