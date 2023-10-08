using System.Collections.Generic;
using TravelLove.DTO;

namespace TravelLove.DTOs
{
    public class BusSeatAddRequestDTO
    {
        public int BusID { get; set; }
        public List<SeatTypeRequest> SeatTypes { get; set; }
    }

}
