using HotelBooking.DTOs;
using HotelBooking.DTOs.Request;
using HotelBooking.Models;

namespace HotelBooking.Interfaces;

public interface IHotelService
{
    Task<Result<List<HotelRoomInfoDTO>>> FindHotelsByNameAsync(string name);
}

public interface IRoomService
{
    Task<Result<Room>> GetRoomByIdAsync(int roomId);
    Task<Result<IEnumerable<RoomAvailabilityInfoDTO>>> GetAvailableRoomsAsync(DateOnly startDate, DateOnly endDate, int guestCount);
    Result IsCapacityValid(Room? room, int guestCount);
    Task<Result> IsRoomAvailableAsync(int roomId, DateOnly startDate, DateOnly endDate);
  
}

public interface IBookingService
{
    Task<Result<Guid>> MakeBookingAsync(BookingRequest booking);
    Task<Result<Booking>> GetBookingByReferenceAsync(Guid referenceNumber);
}

public interface IBookingSystemUtilitiesService
{
    Task<Result> SeedDatabaseAsync();
    Task<Result> ResetDatabaseAsync();
}

public interface IBookingValidationService
{
    Task<Result> ValidateRoomAvailabilityAsync(BookingRequest booking);
}

