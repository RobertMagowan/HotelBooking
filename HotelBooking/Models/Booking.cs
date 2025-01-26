namespace HotelBooking.Models;

using Microsoft.EntityFrameworkCore;
[Index(nameof(RoomId))]
[Index(nameof(StartDate))]
[Index(nameof(EndDate))]
public class Booking
{
    public int Id { get; set; }
    public Guid ReferenceNumber { get; set; }
    public int RoomId { get; set; }
    public Room? Room {get; set; } 
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int GuestCount { get; set; }
}


