using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }

        [ForeignKey("Passenger")]
        public int PassengerID { get; set; }
        public Passenger Passenger { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        public int Rating { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string Comments { get; set; }
    }
}