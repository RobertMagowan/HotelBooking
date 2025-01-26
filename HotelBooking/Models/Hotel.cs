using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models;

public class Hotel
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<Room>? Rooms { get; set; }
}