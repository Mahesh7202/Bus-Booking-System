using TravelLove.Models;

namespace TravelLove.DTO
{
    public class BusAssignedDataDTO
    {
        public Bus Bus { get; set; }
        public List<Schedule> Schedules { get; set; }

    }
}
