using FluentValidation;
using HotelBooking.DTOs.Request;

namespace HotelBooking.ApiInputValidation
{
    public class AvailableRoomsRequestValidator : AbstractValidator<AvailableRoomsRequest>
    {
        public AvailableRoomsRequestValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date cannot be greater than end date.");
        }
    }
}
