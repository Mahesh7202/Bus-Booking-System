using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class Passenger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PassengerID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [Column(TypeName = "varchar(15)")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Column(TypeName = "varchar(10)")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        public int Age { get; set; }
    }
}
