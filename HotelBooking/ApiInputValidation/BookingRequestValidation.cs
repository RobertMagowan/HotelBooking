namespace HotelBooking.ApiInputValidation;
using FluentValidation;
using HotelBooking.DTOs.Request;

public class BookingRequestValidation : AbstractValidator<BookingRequest>
{
    public BookingRequestValidation() 
    {
        RuleFor(bookingRequest => bookingRequest.StartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date must be today or in the future.");

        RuleFor(bookingRequest => bookingRequest.EndDate)
            .Must((bookingRequest, endDate) => endDate > bookingRequest.StartDate)
            .WithMessage("End date must be greater than the start date.");

        RuleFor(bookingRequest => bookingRequest.GuestCount)
            .GreaterThan(0)
            .WithMessage("Guest count must be greater than zero.");
    }
}
