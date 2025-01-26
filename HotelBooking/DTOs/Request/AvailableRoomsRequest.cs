namespace HotelBooking.DTOs.Request
{
    public class AvailableRoomsRequest
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int GuestCount { get; set; }
    }
}
