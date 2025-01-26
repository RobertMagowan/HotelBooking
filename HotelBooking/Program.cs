using HotelBooking.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using HotelBooking.Services;
using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using HotelBooking.DTOs.Request;
using HotelBooking.ApiInputValidation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelContext")));

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IBookingValidationService, BookingValidationService>();
builder.Services.AddScoped<IBookingSystemUtilitiesService, BookingSystemUtilitiesService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BookingRequestValidation>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Booking API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/utilities/seed", async (IBookingSystemUtilitiesService bookingSystemUtilitiesService) =>
{
    return await bookingSystemUtilitiesService.SeedDatabaseAsync().ToResultAsync();
});

app.MapPost("/api/utilities/reset", async (IBookingSystemUtilitiesService bookingSystemUtilitiesService) =>
{
    return await bookingSystemUtilitiesService.ResetDatabaseAsync().ToResultAsync();
});

app.MapGet("/api/hotels", async (string name, IHotelService hotelService) =>
{
    return await hotelService.FindHotelsByNameAsync(name).ToResultAsync(); 
});

app.MapPost("/api/bookings", async (BookingRequest bookingRequest, IBookingService bookingService,  IValidator<BookingRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(bookingRequest);
    return validationResult.IsValid
        ? await bookingService.MakeBookingAsync(bookingRequest).ToResultAsync()
        : Results.BadRequest(validationResult.Errors);
});

app.MapGet("/api/bookings/{referenceNumber}", async (Guid referenceNumber, IBookingService bookingService) =>
{
    return await bookingService.GetBookingByReferenceAsync(referenceNumber).ToResultAsync(); 
});

app.MapGet("/api/rooms/available", async (DateOnly startDate, DateOnly endDate, int guestCount, IRoomService roomService, IValidator<AvailableRoomsRequest> validator) =>
 {
     var request = new AvailableRoomsRequest
     {
         StartDate = startDate,
         EndDate = endDate,
         GuestCount = guestCount
     };

     var validationResult = await validator.ValidateAsync(request);
     return validationResult.IsValid 
           ? await roomService.GetAvailableRoomsAsync(startDate, endDate, guestCount).ToResultAsync()
             : Results.BadRequest(validationResult.Errors);
});

app.Run();

