using PaymentGateway.Domain;

namespace PaymentGateway.Api.Models
{
    public class GetPaymentResponse 
    {
        public string CardNumber { get; set; }

        public string NameOnCard { get; set; }
    }
}
