using FluentNHibernate.Conventions.Inspections;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
	public class Seat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SeatID { get; set; }

        public int ScheduleID { get; set; }

        public int BusID { get; set; }
        public Bus Bus { get; set; }


        [Required(ErrorMessage = "Seat number is required.")]
		[Column(TypeName = "varchar(10)")]
		public string SeatNumber { get; set; }

		[Required(ErrorMessage = "Category is required.")]
		[Column(TypeName = "varchar(20)")]
		public string Category { get; set; }

		[Required(ErrorMessage = "Price is required.")]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Price { get; set; }

		[Column(TypeName = "varchar(100)")]
		public string status { get; set; }
	}
}
