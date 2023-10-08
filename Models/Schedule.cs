using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleID { get; set; }

        [ForeignKey("Bus")]
        public int BusID { get; set; }
        public Bus Bus { get; set; }


        [ForeignKey("Driver")]
        public int DriverID { get; set; }
        public Driver Driver { get; set; }

        [ForeignKey("BusPath")]
        public int PathID { get; set; }
        public BusPath BusPath { get; set; }

        [Required(ErrorMessage = "Departure time is required.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required.")]
        public DateTime ArrivalTime { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; }
    }

}
