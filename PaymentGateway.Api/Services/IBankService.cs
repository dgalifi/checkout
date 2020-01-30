using PaymentGateway.Domain;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public interface IBankService
    {
        Task<BankPaymentResult> ProcessPaymentAsync(string encryptedPayment);
    }
}