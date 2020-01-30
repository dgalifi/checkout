using PaymentGateway.Api.Validation;
using PaymentGateway.Domain;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models
{
    public class MakePaymentRequest
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [ExpiryDateValidation]
        public string ExpiryDate { get; set; }

        [Required]
        public decimal? Amount { get; set; }

        [Required]
        public Currency? Currency { get; set; }

        [Required]
        
        [RegularExpression(@"^\d{3}$", ErrorMessage="Cvv is not valid")]
        public string Cvv { get; set; }

        [Required]
        public string NameOnCard { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
