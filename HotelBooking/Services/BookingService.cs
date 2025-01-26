namespace HotelBooking.Services;

using HotelBooking.DbContext;
using HotelBooking.DTOs;
using HotelBooking.DTOs.Request;
using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

public class BookingService : IBookingService
{
    private readonly HotelContext _context;
    private readonly IBookingValidationService _bookingValidationService;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        HotelContext context,
        IBookingValidationService bookingValidationService,
        ILogger<BookingService> logger)
    {
        _context = context;
        _bookingValidationService = bookingValidationService;
        _logger = logger;
    }

    public async Task<Result<Guid>> MakeBookingAsync(BookingRequest bookingReqeust)
    {
        try
        {
            var validationResult = await _bookingValidationService.ValidateRoomAvailabilityAsync(bookingReqeust);
            if (!validationResult.IsSuccess)
            {
                return Result.CreateError<Guid>(validationResult.ErrorMessage ?? "Unknown Error While Creating Booking", StatusCodes.Status400BadRequest);
            }
 
            var booking = new Booking
            {
                ReferenceNumber = Guid.NewGuid(),
                RoomId = bookingReqeust.RoomId,  
                StartDate = bookingReqeust.StartDate,
                EndDate = bookingReqeust.EndDate,
                GuestCount = bookingReqeust.GuestCount
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Result.CreateSuccess(booking.ReferenceNumber);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating booking");
            return Result.CreateError<Guid>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result<Booking>> GetBookingByReferenceAsync(Guid referenceNumber)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.ReferenceNumber == referenceNumber);

            return booking != null
                ? Result.CreateSuccess(booking)
                : Result.CreateError<Booking>("Booking not found", StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving booking");
            return Result.CreateError<Booking>("An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }
}
