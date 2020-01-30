using System;

namespace PaymentGateway.Domain
{
    public class BankPaymentResult
    {
        public Guid Id { get; set; }

        public bool Success { get; set; }
    }
}
