using PaymentGateway.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.Domain
{
    public class DataRepository : IDataRepository
    {
        private static IList<Payment> _paymentDb;

        public DataRepository()
        {
            _paymentDb = new List<Payment>();
        }

        public Payment GetPayment(Guid guid)
        {
            try
            {
                return _paymentDb.FirstOrDefault(x => x.Id == guid);
            }
            catch (Exception)
            {
                throw new DataRepoException();
            }
        }

        public void SavePayment(Payment payment)
        {
            try
            {
                _paymentDb.Add(payment);
            }
            catch (Exception)
            {
                throw new DataRepoException();
            }
        }
    }
}
