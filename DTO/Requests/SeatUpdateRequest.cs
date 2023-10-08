namespace TravelLove.DTO.Requests
{
    public class SeatUpdateRequest
    {
        public int scheduleId { get; set; }
        public int busId { get; set; }
        public List<string> seatNumbers { get; set; }
    }
}
