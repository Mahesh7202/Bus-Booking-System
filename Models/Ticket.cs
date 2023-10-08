using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketID { get; set; }


        [ForeignKey("Passenger")]
        public int PassengerID { get; set; }
        public Passenger Passenger { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }

        [Required(ErrorMessage = "Seat number is required.")]
        public string SeatNumber { get; set; }

        [Required(ErrorMessage = "Fare amount is required.")]
        public decimal FareAmount { get; set; }

        [Required(ErrorMessage = "Booking time is required.")]
        public DateTime BookingTime { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; }

     
    }

}