using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Tests.Unit
{
    public class PaymentsControllerTests
    {

        private PaymentsController _controller;
        private Mock<ILogger<PaymentsController>> _logger;
        private Mock<IBankService> _bankService;
        private Mock<IDataRepository> _dataRepository;
        private Mock<IEncryption> _encryption;

        private MakePaymentRequest _request;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<PaymentsController>>();
            _bankService = new Mock<IBankService>();
            _dataRepository = new Mock<IDataRepository>();
            _encryption = new Mock<IEncryption>();
            _controller = new PaymentsController(_bankService.Object, _dataRepository.Object, _encryption.Object, _logger.Object);


            _request = new MakePaymentRequest
            {
                CardNumber = "4024-0071-7669-1425",
                Amount = 20,
                Currency = Currency.GBP,
                Cvv = "123",
                ExpiryDate = "10/25",
                NameOnCard = "AnyName",
                Address = "AnyAddress"
            };
        }


        [Test]
        public async Task Make_ShouldCallEncryptWithPaymentMappedFromRequest()
        {
            // Given 
            _bankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<string>())).ReturnsAsync(new BankPaymentResult());

            _encryption.Setup(x => x.Encrypt(It.IsAny<Payment>())).Verifiable();

            // When
            await _controller.Make(_request);

            //Then
            _encryption.Verify(x => x.Encrypt(It.Is<Payment>(x =>
            x.Amount == _request.Amount &&
            x.CardNumber == _request.CardNumber &&
            x.Currency == _request.Currency &&
            x.Cvv == _request.Cvv &&
            x.ExpiryDate == _request.ExpiryDate &&
            x.Address == _request.Address &&
            x.NameOnCard == _request.NameOnCard)), Times.Once);
        }

        [Test]
        public async Task Make_ShouldProcessPayment_WithEncryptedPayment()
        {
            // Given 
            var encryptedString = "this is an encrypted string";
            _bankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<string>())).ReturnsAsync(new BankPaymentResult());

            _encryption.Setup(x => x.Encrypt(It.IsAny<Payment>())).Returns(encryptedString);

            // When
            await _controller.Make(_request);

            // Then
            _bankService.Verify(x => x.ProcessPaymentAsync(encryptedString), Times.Once);
        }

        [Test]
        public async Task Make_ShouldReturn500WhenBankDeniesPayment()
        {
            // Given 
            var result = new BankPaymentResult { Success = false };

            _bankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<string>())).ReturnsAsync(result);

            // When
            var response = await _controller.Make(_request);

            // Then
            Assert.IsInstanceOf<ObjectResult>(response);
            Assert.AreEqual(500, (response as ObjectResult).StatusCode);

            var obj = (response as ObjectResult).Value as MakePaymentResponse;
            Assert.AreEqual(new Guid(), obj.Id);
            Assert.AreEqual("AcquiringBank has rejected the payment", obj.Message);
            Assert.IsFalse(obj.SuccessfulPayment);
        }

        [Test]
        public async Task Make_ShouldSaveinDbWhenBankApprovesPayment()
        {
            // Given 
            var id = Guid.NewGuid();
            var result = new BankPaymentResult { Success = true, Id = id };

            _dataRepository.Setup(x => x.SavePayment(It.IsAny<Payment>())).Verifiable();
            _bankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<string>())).ReturnsAsync(result);

            // When
            var response = await _controller.Make(_request);

            // Then
            _dataRepository.Verify(x => x.SavePayment(It.Is<Payment>(x =>
            x.Id == result.Id &&
            x.Amount == _request.Amount &&
            x.CardNumber == _request.CardNumber &&
            x.Currency == _request.Currency &&
            x.Cvv == _request.Cvv &&
            x.ExpiryDate == _request.ExpiryDate &&
            x.Address == _request.Address &&
            x.NameOnCard == _request.NameOnCard)), Times.Once);
        }

        [Test]
        public async Task Make_ShouldReturn200WhenItsAllGood()
        {
            // Given 
            var id = Guid.NewGuid();
            var result = new BankPaymentResult { Success = true, Id = id };

            _bankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<string>())).ReturnsAsync(result);

            // When
            var response = await _controller.Make(_request);

            // Then
            Assert.IsInstanceOf<OkObjectResult>(response);
            var obj = (response as OkObjectResult).Value as MakePaymentResponse;
            Assert.AreEqual(id, obj.Id);
            Assert.AreEqual("ok", obj.Message);
            Assert.IsTrue(obj.SuccessfulPayment);
        }

        [Test]
        public async Task Make_ShouldReturn200WhenDataSavingFailsAndLog()
        {
            // Given 
            var id = Guid.NewGuid();
            var result = new BankPaymentResult { Success = true, Id = id };

            _bankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<string>())).ReturnsAsync(result);
            _dataRepository.Setup(x => x.SavePayment(It.IsAny<Payment>())).Throws(new DataRepoException());

            // When
            var response = await _controller.Make(_request);

            // Then
            Assert.IsInstanceOf<OkObjectResult>(response);
            var obj = (response as OkObjectResult).Value as MakePaymentResponse;
            Assert.AreEqual(id, obj.Id);
            Assert.AreEqual("ok", obj.Message);
            Assert.IsTrue(obj.SuccessfulPayment);

            _logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((object v, Type _) => v.ToString().Contains("error saving data")),
                It.IsAny<DataRepoException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once, "action is not logging properly");
        }


        [Test]
        public void Get_ShouldGetDataFromRepo()
        {
            // Given 
            var id = Guid.NewGuid();

            _dataRepository.Setup(x => x.GetPayment(It.IsAny<Guid>())).Returns(new Payment
            {
                CardNumber = "1234-1234-1234-1234"
            });

            // When
            var response = _controller.Get(id);

            // Then
            _dataRepository.Verify(x => x.GetPayment(id), Times.Once);
        }

        [Test]
        public void Get_ShouldReturn200()
        {
            // Given 
            var id = Guid.NewGuid();
            var payment = new Payment
            {
                CardNumber = "4024-0071-7669-1425",
                Amount = 20,
                Currency = Currency.GBP,
                Cvv = "123",
                ExpiryDate = "10/25",
                NameOnCard = "AnyName",
                Address = "AnyAddress"
            };

            _dataRepository.Setup(x => x.GetPayment(It.IsAny<Guid>())).Returns(payment);

            // When
            var response = _controller.Get(id);
            Assert.IsInstanceOf<OkObjectResult>(response);
            var obj = (response as OkObjectResult).Value as GetPaymentResponse;

            // Then
            Assert.AreEqual($"xxxx-xxxx-xxxx-{payment.CardNumber.Substring(payment.CardNumber.Length - 4)}", obj.CardNumber);
            Assert.AreEqual(payment.NameOnCard, obj.NameOnCard);
        }
    }
}