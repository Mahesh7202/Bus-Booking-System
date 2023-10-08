using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class BusPath
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PathID { get; set; }

        [Required(ErrorMessage = "BusTravel name is required.")]
        [Column(TypeName = "varchar(100)")]
        public string PathName { get; set; }

        [Required(ErrorMessage = "Starting point is required.")]
        [Column(TypeName = "varchar(100)")]
        public string StartingPoint { get; set; }

        [Required(ErrorMessage = "Ending point is required.")]
        [Column(TypeName = "varchar(100)")]
        public string EndingPoint { get; set; }

        [Required(ErrorMessage = "Distance is required.")]
        public double Distance { get; set; }
    }


    
}