namespace HotelBooking.Services;

using HotelBooking.DbContext;
using HotelBooking.DTOs;
using HotelBooking.DTOs.Request;
using HotelBooking.Interfaces;

public class BookingValidationService : IBookingValidationService
{
    private readonly HotelContext _context;
    private readonly IRoomService _roomService;
    private readonly ILogger<BookingValidationService> _logger;

    public BookingValidationService(
        HotelContext context,
        IRoomService roomService,
        ILogger<BookingValidationService> logger)
    {
        _context = context;
        _roomService = roomService;
        _logger = logger;
    }

    public async Task<Result> ValidateRoomAvailabilityAsync(BookingRequest booking)
    {
        try
        {
            var roomResult = await _roomService.GetRoomByIdAsync(booking.RoomId);
            if (!roomResult.IsSuccess)
            {
                _logger.LogWarning("Room validation failed: {ErrorMessage}", roomResult.ErrorMessage);
                return Result.CreateError(roomResult.ErrorMessage ?? "Unknown Error", StatusCodes.Status404NotFound);
            }

            var room = roomResult.Data;

            if (!_roomService.IsCapacityValid(room, booking.GuestCount).IsSuccess)
            {
                _logger.LogWarning("Room capacity exceeded for room {RoomId}", booking.RoomId);
                return Result.CreateError("Room capacity exceeded", StatusCodes.Status400BadRequest);
            }
            
            var availableResult = await _roomService.IsRoomAvailableAsync(booking.RoomId, booking.StartDate, booking.EndDate);
            if (!availableResult.IsSuccess)
            {
                _logger.LogWarning("Room {RoomId} not available for selected dates", booking.RoomId);
                return Result.CreateError("Room is not available for the selected dates", StatusCodes.Status400BadRequest);
            }

            return Result.CreateSuccess();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while validating room availability");
            return Result.CreateError("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }
}
