using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TravelLove.Models;

namespace TravelLove.DTO.Requests
{
    public class BookingRequest
    {
          
            public int UserID { get; set; }

            public DateTime BookingTime { get; set; }

            public int? CreditCardID { get; set; }
            public CreditCard CreditCard { get; set; }


            public decimal TotalAmount { get; set; }
            public List<int> TicketIDs { get; set; }


    }

}
