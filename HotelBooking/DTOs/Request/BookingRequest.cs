namespace HotelBooking.DTOs.Request;

public class BookingRequest
{
    public int RoomId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int GuestCount { get; set; }
}
