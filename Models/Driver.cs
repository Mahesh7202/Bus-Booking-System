using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
	public class Driver
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DriverID { get; set; }

		[Required(ErrorMessage = "First name is required.")]
		[Column(TypeName = "varchar(50)")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required.")]
		[Column(TypeName = "varchar(50)")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Contact number is required.")]
		[Column(TypeName = "varchar(15)")]
		public string ContactNumber { get; set; }

		[Required(ErrorMessage = "License number is required.")]
		[Column(TypeName = "varchar(20)")]
		public string LicenseNumber { get; set; }
	}

}