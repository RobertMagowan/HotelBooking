using HotelBooking.DbContext;
using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services;

public class HotelService : IHotelService
{
    private readonly HotelContext _context;
    private readonly ILogger<HotelService> _logger;

    public HotelService(HotelContext context, ILogger<HotelService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<HotelRoomInfoDTO>>> FindHotelsByNameAsync(string name)
    { 
        try
        {
            var hotels = await _context.Hotels
                .Where(h => EF.Functions.Like(h.Name, $"%{name}%"))
                .Include(x => x.Rooms!)
                .ThenInclude(x => x.RoomType)
                .Select(hotel => new HotelRoomInfoDTO
                {
                    HotelName = hotel.Name,
                    Rooms = hotel.Rooms!
                        .Where(room => room != null)
                        .Select(room => new RoomInfoDTO
                        {
                            RoomNumber = room.Id,
                            Capacity = room.RoomType.Capacity
                        })
                })
                .ToListAsync();

            return hotels.Any()
                ? Result.CreateSuccess(hotels)
                : Result.CreateError<List<HotelRoomInfoDTO>>("No hotels found", StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while finding hotels by name");
            return Result.CreateError<List<HotelRoomInfoDTO>>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }
}

