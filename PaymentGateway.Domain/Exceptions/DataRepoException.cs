using System;
using System.Runtime.Serialization;

namespace PaymentGateway.Domain.Exceptions
{
    [Serializable]
    public class DataRepoException : Exception
    {
        public DataRepoException()
        {
        }

        public DataRepoException(string message) : base(message)
        {
        }

        public DataRepoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataRepoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}