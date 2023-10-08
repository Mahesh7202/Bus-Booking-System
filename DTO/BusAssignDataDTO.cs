using System;

namespace TravelLove.DTOs
{
    public class BusAssignDataDTO
    {
        public int BusID { get; set; }
        public string LicensePlate { get; set; }
        public string CurrentLocation { get; set; }
        public int UpperSeatsCount { get; set; }
        public int LowerSeatsCount { get; set; }
        public int Capacity { get; set; }
        public string DriverName { get; set; }
        public string PathName { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; }
    }
}
