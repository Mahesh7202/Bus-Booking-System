namespace TravelLove.DTO.Requests
{
    public class BookingEmailRequest
    {
        
            public List<BookingData> BookingDataList { get; set; }
            public List<string> EmailIds { get; set; }
        
    }
}
