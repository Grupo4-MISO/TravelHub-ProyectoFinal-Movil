using App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.DTOs
{
    public class ReservationTemporalDTO
    {
        public ReservationCreateResponseDto? Booking { get; set; }
        public AccommodationDetailDto? Property { get; set; }
        public AccommodationDetailRoomDto? Room { get; set; }
        public Traveler? Traveler { get; set; }
        public DateTime CheckIn {  get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
