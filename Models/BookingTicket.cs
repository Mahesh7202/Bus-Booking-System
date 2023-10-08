using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class BookingTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingTicketID { get; set; }

        [ForeignKey("Booking")]
        public int BookingID { get; set; }
        public Booking Booking { get; set; }

        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }
    }
}
