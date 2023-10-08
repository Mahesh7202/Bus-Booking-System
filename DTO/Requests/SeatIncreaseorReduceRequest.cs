namespace TravelLove.DTO.Requests
{
    public class SeatIncreaseorReduceRequest
    {
        public int BusId { get; set; }
        public List<string> SeatTypes { get; set; }
    }
}
