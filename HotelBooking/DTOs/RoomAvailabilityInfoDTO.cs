namespace HotelBooking.DTOs
{
    public class RoomAvailabilityInfoDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public required string RoomType { get; set; }
        public int Capacity { get; set; }
        public required string HotelName { get; set; }
    }
}
