using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLove.Models
{
    public class CreditCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardID { get; set; }

        [ForeignKey("UserAccount")]
        public int UserID { get; set; }
        public UserAccount UserAccount { get; set; }

        [Column(TypeName = "varchar(50)")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card Number must be 16 digits.")]
        [Required(ErrorMessage = "Enter the 16 digits of your card number.")]
        public string CardNumber { get; set; }

    

        [Required(ErrorMessage = "Card expiration year is required.")]
        public string ExpireYear { get; set; }

        [Required(ErrorMessage = "Card expiration month is required.")]
        public string ExpireMonth { get; set; }

        [Required(ErrorMessage = "CVC is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVC must be 3 digits.")]
        public string Cvc { get; set; }

        [Required(ErrorMessage = "Cardholder's full name is required.")]
        public string CardHolderFullName { get; set; }

        [Required(ErrorMessage = "Balance is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Balance must be greater than zero.")]
        public decimal Balance { get; set; }
    }
}
