namespace HotelBooking.DTOs;

public class HotelRoomInfoDTO
{
    public int Id { get; set; }
    public required string HotelName { get; set; }
    public required IEnumerable<RoomInfoDTO> Rooms { get; set; }
}

public class RoomInfoDTO
{
    public int Id { get; set; }
    public int RoomNumber { get; set; }
    public int Capacity { get; set; }
}
