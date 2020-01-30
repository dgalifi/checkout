using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IBankService _bankService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IDataRepository _dataRepository;
        private readonly IEncryption _encryption;

        public PaymentsController(IBankService bankService, IDataRepository dataRepository, IEncryption encryption, ILogger<PaymentsController> logger)
        {
            _bankService = bankService;
            _dataRepository = dataRepository;
            _encryption = encryption;
            _logger = logger;
        }

        [HttpPost("Make")]
        [Authorize]
        public async Task<IActionResult> Make([FromBody]MakePaymentRequest request)
        {
            var payment = PaymentFromRequest(request);

            var encryptedPayment = _encryption.Encrypt(payment);

            // calling acquiring bank for approval
            var bankResponse = await _bankService.ProcessPaymentAsync(encryptedPayment);
            var response = ToMakePaymentResponse(bankResponse);

            if (!response.SuccessfulPayment)
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            payment.Id = response.Id;

            try
            {
                _dataRepository.SavePayment(payment);
            }

            catch (DataRepoException ex)
            {
                _logger.LogError(ex, "error saving data");

                // in here there should be some sort of re-trying mechanism or dead letter queue in order to save the Payment
                // Payment is successful so returns 200
            }

            return Ok(response);
        }

        [HttpGet("Get/{id}")]
        [Authorize]
        public IActionResult Get(Guid id)
        {
            try
            {
                var paymentDetails = _dataRepository.GetPayment(id);
                return Ok(ToGetPaymentResponse(paymentDetails));
            }
            catch (DataRepoException ex)
            {
                _logger.LogError(ex, "error getting data");

                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "error getting data" });
            }
        }

        #region Mapping methods

        private Payment PaymentFromRequest(MakePaymentRequest request)
        {
            return new Payment
            {
                Amount = request.Amount,
                CardNumber = request.CardNumber,
                Currency = request.Currency,
                Cvv = request.Cvv,
                ExpiryDate = request.ExpiryDate,
                Address = request.Address,
                NameOnCard = request.NameOnCard
            };
        }

        private GetPaymentResponse ToGetPaymentResponse(Payment payment)
        {
            return new GetPaymentResponse
            {
                CardNumber = $"xxxx-xxxx-xxxx-{payment.CardNumber.Substring(payment.CardNumber.Length - 4)}",
                NameOnCard = payment.NameOnCard
            };
        }


        private MakePaymentResponse ToMakePaymentResponse(BankPaymentResult bankPaymentResult)
        {
            return new MakePaymentResponse
            {
                Id = bankPaymentResult.Id,
                SuccessfulPayment = bankPaymentResult.Success,
                Message = bankPaymentResult.Success ? "ok" : "AcquiringBank has rejected the payment"
            };
        }
        #endregion
    }
}
