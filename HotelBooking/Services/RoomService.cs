namespace HotelBooking.Services;

using HotelBooking.DbContext;
using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

public class RoomService : IRoomService
{
    private readonly HotelContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(HotelContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Room>> GetRoomByIdAsync(int roomId)
    {
        try
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            return room != null
                ? Result.CreateSuccess(room)
                : Result.CreateError<Room>("Room not found", StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving room by ID");
            return Result.CreateError<Room>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result<IEnumerable<RoomAvailabilityInfoDTO>>> GetAvailableRoomsAsync(DateOnly startDate, DateOnly endDate, int guestCount)
    {
        try
        {
            var roomsQuery = GetAvailableRoomsQuery(startDate, endDate)
                .Include(r => r.RoomType)
                .Include(r => r.Hotel)
                .Where(r => r.RoomType.Capacity >= guestCount)
                .Select(room => new RoomAvailabilityInfoDTO
                {
                    Id = room.Id,
                    RoomId = room.Id,
                    RoomNumber = room.RoomNumber,
                    RoomType = room.RoomType.Type,
                    Capacity = room.RoomType.Capacity,
                    HotelName = room.Hotel.Name
                });

            var rooms = await roomsQuery.ToListAsync();

            return rooms.Count > 0
                ? Result.CreateSuccess<IEnumerable<RoomAvailabilityInfoDTO>>(rooms)
                : Result.CreateError<IEnumerable<RoomAvailabilityInfoDTO>>("No available rooms found", StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving available rooms");
            return Result.CreateError<IEnumerable<RoomAvailabilityInfoDTO>>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result> IsRoomAvailableAsync(int roomId, DateOnly startDate, DateOnly endDate)
    {
        try
        {
            var roomsQuery = GetAvailableRoomsQuery(startDate, endDate).Where(room => room.Id == roomId);
            var isAvailable = await roomsQuery.AnyAsync();

            return isAvailable
                ? Result.CreateSuccess(true)
                : Result.CreateError<bool>("Room is not available for the selected dates", StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while checking room availability");
            return Result.CreateError<bool>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public Result IsCapacityValid(Room? room, int guestCount)
    {
        try
        {
            return room?.RoomType?.Capacity >= guestCount
                ? Result.CreateSuccess(true)
                : Result.CreateError<bool>("Room capacity is insufficient for the guest count", StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while checking room capacity");
            return Result.CreateError<bool>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public IQueryable<Room> GetAvailableRoomsQuery(DateOnly startDate, DateOnly endDate)
    {
        return _context.Rooms
            .Where(room =>
                !_context.Bookings.Any(b =>
                    b.RoomId == room.Id &&
                    b.StartDate <= endDate &&
                    b.EndDate > startDate));
    }
}
