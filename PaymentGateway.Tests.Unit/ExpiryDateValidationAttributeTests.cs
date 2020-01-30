using NUnit.Framework;
using PaymentGateway.Api.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaymentGateway.Tests.Unit
{
    [TestFixture]
    public class ExpiryDateValidationAttributeTests
    {
        private ExpiryDateValidationAttribute _attribute;

        [SetUp]
        public void Setup()
        {
            _attribute = new ExpiryDateValidationAttribute();
        }

        [Test]
        public void Valid()
        {
            // When
            var res = _attribute.IsValid($"10/{(DateTime.Now.Year + 1).ToString().Substring(2, 2)}");

            Assert.IsTrue(res);
        }

        [TestCase("10/2021")]
        [TestCase("")]
        [TestCase("asdagsh")]
        public void WrongFormat(string date)
        {
            // When
            var res = _attribute.IsValid(date);

            Assert.IsFalse(res);
        }

        [TestCase("10/2019")]
        public void DateInThePast(string date)
        {
            // When
            var res = _attribute.IsValid("10/2019");

            Assert.IsFalse(res);
        }
    }
}
