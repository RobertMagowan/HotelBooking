namespace HotelBooking.Services;

using HotelBooking.DbContext;
using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

public class BookingSystemUtilitiesService : IBookingSystemUtilitiesService
{
    private readonly HotelContext _context;
    private readonly ILogger<BookingSystemUtilitiesService> _logger;

    public BookingSystemUtilitiesService(HotelContext context, ILogger<BookingSystemUtilitiesService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> SeedDatabaseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();

            if (await _context.Hotels.AnyAsync())
            {
                _logger.LogWarning("Database already seeded");
                return Result.CreateError("Database already seeded", StatusCodes.Status400BadRequest);
            }

            var hotels = new List<Hotel>
            {
                new() { Id = 1, Name = "Europa Hotel Belfast" },
                new() { Id = 2 ,Name = "The Merchant Hotel" }
            };

            var roomTypes = new List<RoomType>
            {
                new() { Id=1, Type = "Single", Capacity = 1 },
                new() { Id=2, Type = "Double", Capacity = 2 },
                new() { Id=3, Type = "Deluxe", Capacity = 4 }
            };

            var rooms = new List<Room>
            {
                // Europa Hotel Belfast rooms
                new() { Id = 1,  RoomType = roomTypes[0], Hotel = hotels[0], RoomNumber = 101 },
                new() { Id = 2,  RoomType = roomTypes[1], Hotel = hotels[0], RoomNumber = 102 },
                new() { Id = 3,  RoomType = roomTypes[2], Hotel = hotels[0], RoomNumber = 103 },
                new() { Id = 4,  RoomType = roomTypes[1], Hotel = hotels[0], RoomNumber = 104 },
                new() { Id = 5,  RoomType = roomTypes[0], Hotel = hotels[0], RoomNumber = 105 },
                new() { Id = 6,  RoomType = roomTypes[2], Hotel = hotels[0], RoomNumber = 106 },

                // The Merchant Hotel rooms
                new() { Id = 7,  RoomType = roomTypes[0], Hotel = hotels[1], RoomNumber = 101 },
                new() { Id = 8,  RoomType = roomTypes[1], Hotel = hotels[1], RoomNumber = 102 },
                new() { Id = 9,  RoomType = roomTypes[2], Hotel = hotels[1], RoomNumber = 103 },
                new() { Id = 10,  RoomType = roomTypes[1], Hotel = hotels[1], RoomNumber = 104 },
                new() { Id = 11,  RoomType = roomTypes[0], Hotel = hotels[1], RoomNumber = 105 },
                new() { Id = 12,  RoomType = roomTypes[2], Hotel = hotels[1], RoomNumber = 106 }
            };

            _context.Hotels.AddRange(hotels);
            _context.RoomTypes.AddRange(roomTypes);
            _context.Rooms.AddRange(rooms);
           
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeded successfully");
            return Result.CreateSuccess(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while seeding database");
            return Result.CreateError("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result> ResetDatabaseAsync()
    {
        try
        {
            _context.Rooms.RemoveRange(_context.Rooms);
            _context.Bookings.RemoveRange(_context.Bookings);
            _context.Hotels.RemoveRange(_context.Hotels);
            _context.RoomTypes.RemoveRange(_context.RoomTypes);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database reset successfully");
            return Result.CreateSuccess();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while resetting database");
            return Result.CreateError("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }
}
