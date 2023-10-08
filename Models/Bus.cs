using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class Bus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BusID { get; set; }

        [Required(ErrorMessage = "License plate is required.")]
        [Column(TypeName = "varchar(20)")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [Column(TypeName = "varchar(50)")]
        public string Model { get; set; }


        [Required(ErrorMessage = "Type is required.")]
        [Column(TypeName = "varchar(50)")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        public int Capacity { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CurrentLocation { get; set; }


        [ForeignKey("BusSeat")]
        public int? BusSeatID { get; set; }
        public BusSeat BusSeat { get; set; }

        public int UpperSeatsCount { get; set; }
        public int LowerSeatsCount { get; set; }
    }


   
}