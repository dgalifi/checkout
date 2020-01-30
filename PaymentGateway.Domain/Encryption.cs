using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System;

namespace PaymentGateway.Domain
{
    public class Encryption : IEncryption
    {
        private readonly string _publicKey;
        private readonly UnicodeEncoding _encoder;

        public Encryption(IConfiguration config)
        {
            _encoder = new UnicodeEncoding();
            _publicKey = config["publicKey"];
        }

        private RSACryptoServiceProvider GetDecrypterFromPrivateKey(string privateKey)
        {
            AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(new StringReader(privateKey)).ReadObject();

            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Private);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        private RSACryptoServiceProvider GetEncrypterFromPublicKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        public string Encrypt(Payment payment)
        {
            var data = JsonConvert.SerializeObject(payment);

            var rsa = GetEncrypterFromPublicKey(_publicKey);

            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        public string Decrypt(string data, string privateKey)
        {
            var dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            var rsa = GetDecrypterFromPrivateKey(privateKey);

            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }
    }
}
