using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Domain;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public class BankServiceMock : IBankService
    {
        private readonly IEncryption _encryption;
        private readonly ILogger<BankServiceMock> _logger;
        private const string _privateKey =
@"-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA00fLWcTOojFudslOyLeuE6zOEQBjdZLOWg7zxni/AV0mRoou
PtAEHwtG9G0rXLyB4/x79wSne6USg9HSZiMvn4pwTAo86rRJbhF1CqVa6CfOjAby
9PpMthf/GsPWbm7CJ1vJs/n75SiYMBp2ZpsYoRyoCcaK4z0UQPJFaxXQ9GEpCpMw
urAdfiwGPAf1ll89+8Z927EUaXuSdqDzbrJFXghgHHBpVNBhLwI9cHxuPmR9YWJT
M+I+gdtqpxWeLXS4oLRJB9hpJaM5OVnr/9IuA6VBxrFA4vChhsvDlNtUXBw+eufM
AW1M0hlMYTweTUe/5U2A2BJRIUWMqWteMVTRDQIDAQABAoIBAQC3DXP84cwZsHRB
jKaqtqGmwEiFaG3Dtx773gqNgZ4Tg24fHad1mmqHrxCZw2AXZ+et7NpGqYymTt3c
wdTl00R2mM40w4YF34/jQlLCl4NMsvh8T6T+ZUJDXkUTHmvRoDqtORRnz4XUBLje
rH665VOQezCHz7ITsZ6zqnRElFFG+Lgyt7jkCfs04SqkqaUlrSqHU2yGijLHpUrk
zYJfca4mgAnGwSQ8/naoH2u6puzicjMqYB4J/kHcs40xSzDQyDMsGUaSDuNf4iqk
DQhb24XfoUirygNSXafF1//GmYWfI7M7ERKPVBqCIS+u8DsLeKE6fWrRbr6QoHC0
6Ox3vdTtAoGBAOzXdWb0/UalAk7xJR4TYLBzL16BbtDHzkcbfNN6BUvN5xNwL2Rf
9FBkd9XAHHXhsPrxdEbZH6peArZt4Ak70cjx5uE2336UO5G+F7tV4EJgcS//Xxoy
TbjEGycsbMtXM3qkIyqppOfuqTS/folmXU9U4L+ULFAfCPSz5e1FrWaTAoGBAORf
AxAxQCnFKnUhXG3lv4uByqkOsTkBbwdieq3fIjLxSLLPfRmvqesmse/Jyogm0bNV
mUIzHEyqiZ636CfuiNYi0sklN4+F+lCkLNTMUZfWHiScSKDqQp++BknmFW+uwibw
G/ngIJvv0LF/wWesb9S+XTHNx+n0cMUAnlObgQ3fAoGAMgmbPjNIR2KZSEBeTSpe
5SiQu3CxDt8Hz7IY/zzqXYeU6GBtQtGa9lkjkD9UQ/t4vqvAmy9IJ2BRRmWUTZ8b
xU6GLtK/CSkcJMB6lxOfm6Zwg2l6mDhEf2K/MdmK6rLzp1eCLoNE2dNsYD6M505/
LEGMci1F52+HHtvbGfP8nI0CgYEAxO+6cIhIUL0atA+k6ZCq8d9nqQpmgkZePPwL
100I7O1uR+TueOwnmAWyhJp78ZWoWQKEO7ZfvS2DqD/oECeAEUnOhG93zmk3R7uM
faYE5u9a5y9u4nWiJqd0PN4RtlH6VCHTvlQtSswwKCGH46OsfMX9ECnpMDP/26OA
5ZDAZfkCgYBEYXHmarqfpyzaBf6kxE5hpYcmiS6DEQ2Nm9bMJSafJ+ccPNhU9nyY
ix+cU2SAMeBu3N2hYo94qAJIdw1KBBeA8MCBJl6wtOV5t80fEHy3FD98rqmfcCI2
vO29bI+TtoxNsD68BjHhV1AN7CpWOMkVJ433QOfAigkhMf02SWaKIw==
-----END RSA PRIVATE KEY-----";


        public BankServiceMock(IEncryption encryption, ILogger<BankServiceMock> logger)
        {
            _encryption = encryption;
            _logger = logger;
        }

        public Task<BankPaymentResult> ProcessPaymentAsync(string encryptedPayment)
        {
            // This method simulates what the AcquiringBank would normally do.
            // Decrypts the payment Details using the private key and processes the payment
            // For demo purposes the BankServiceMock will allow only payment with a card number starting with 4

            var serialised = _encryption.Decrypt(encryptedPayment, _privateKey);

            var payment = JsonConvert.DeserializeObject<Payment>(serialised);

            var validPayment = payment.CardNumber.StartsWith('4');

            if (!validPayment)
                _logger.LogInformation($"Payment denied on card: {payment.CardNumber}");

            return Task.FromResult(new BankPaymentResult
            {
                Id = validPayment ? Guid.NewGuid() : Guid.Empty,
                Success = validPayment
            });
        }
    }
}