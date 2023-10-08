using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class BusSeat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BusSeatID { get; set; }

        [Required(ErrorMessage = "Bus is required.")]
        public int BusID { get; set; }

        [Required(ErrorMessage = "Seat type is required.")]
        [Column(TypeName = "varchar(50)")]
        public string SeatType { get; set; }

        [Required(ErrorMessage = "Total seats is required.")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Seats available is required.")]
        public int SeatsAvailable { get; set; }

        // Navigation property to link BusSeat to the associated Bus
        public Bus Bus { get; set; }
    }
}
