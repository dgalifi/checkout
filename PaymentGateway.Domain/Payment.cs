using Newtonsoft.Json;
using System;

namespace PaymentGateway.Domain
{
    public class Payment
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public decimal? Amount { get; set; }

        public Currency? Currency { get; set; }

        public string Cvv { get; set; }

        [JsonIgnore]
        public string NameOnCard { get; set; }

        [JsonIgnore]
        public string Address { get; set; }
    }
}
