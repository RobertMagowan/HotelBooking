using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models;

[Index(nameof(HotelId), nameof(RoomNumber), IsUnique = true)]
public class Room
{

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public int RoomNumber { get; set; }

    public int RoomTypeId { get; set; }

    public required RoomType RoomType { get; set; }

    public int HotelId { get; set; }

    public required Hotel Hotel { get; set; }
}