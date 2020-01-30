using System;
using System.Linq;

namespace PaymentGateway.Domain
{
    public interface IDataRepository
    {
        Payment GetPayment(Guid guid);

        void SavePayment(Payment payment);
    }
}
