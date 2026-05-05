using App.DTOs;
using App.Responses;

namespace App.Services.Interfaces;

public interface IBookingService
{
    Task<HttpResponseWrapper<List<BookingResponseDto>>> GetUserBookingsAsync(string userId);
    Task<HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>> GetHotelsByRoomIdsAsync(List<string> roomIds);
    Task<HttpResponseWrapper<object>> CreateReservationHoldAsync(ReservationHoldRequestDto payload);
}
