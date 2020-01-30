using System;

namespace PaymentGateway.Api.Models
{
    public class MakePaymentResponse
    {
        public Guid Id { get; set; }

        public bool SuccessfulPayment { get; set; }

        public string Message { get; set; }
    }
}
