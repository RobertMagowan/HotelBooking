using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models;
public class RoomType
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public required string Type { get; set; }
    public int Capacity { get; set; }
}


