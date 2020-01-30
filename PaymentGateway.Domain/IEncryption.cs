namespace PaymentGateway.Domain
{
    public interface IEncryption
    {
        string Encrypt(Payment payment);

        string Decrypt(string data, string privateKey);
    }
}