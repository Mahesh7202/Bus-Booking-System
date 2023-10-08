using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelLove.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingID { get; set; }

        [ForeignKey("UserAccount")]
        public int UserID { get; set; }
        public UserAccount UserAccount { get; set; }

        [Required(ErrorMessage = "Booking time is required.")]
        public DateTime BookingTime { get; set; }

        [ForeignKey("CreditCard")]
        public int? CreditCardID { get; set; }
        public CreditCard CreditCard { get; set; }


        [Required(ErrorMessage = "Total amount is required.")]
        public decimal TotalAmount { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }

}
