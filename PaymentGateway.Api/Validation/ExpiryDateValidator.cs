using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Validation
{
    public class ExpiryDateValidationAttribute : ValidationAttribute
    {
        public string FormatErrorMessage() =>
            $"Expiry date is in the wrong format";

        public string FutureErrorMessage() =>
            $"Expiry date must be in the future";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var expiryDate = (string)value;

            if (string.IsNullOrEmpty(expiryDate) || expiryDate.Length != 5 || expiryDate[2] != '/')
                return new ValidationResult(FormatErrorMessage());

            var month = int.Parse(expiryDate.Substring(0, 2));
            var year = int.Parse($"20{expiryDate.Substring(3, 2)}");

            var date = new DateTime(year, month, 1);

            if (date < DateTime.Now)
                return new ValidationResult(FutureErrorMessage());

            return ValidationResult.Success;
        }
    }
}