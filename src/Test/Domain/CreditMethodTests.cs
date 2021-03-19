using NUnit.Framework;
using Moneybox.App;
using System;

namespace Test.Domain
{
    [TestFixture]
    public class CreditMethodTests
    {
        [TestCase(2000, 2001)]
        [TestCase(2001, 2000)]
        [TestCase(0, 4001)]
        [TestCase(4001, 0)]
        [TestCase(5001, -1000)]
        [TestCase(-1000, 5001)]
        [TestCase(19999, 999)]
        public void GivenInvalidCombinations_WhenCreditMethodIsCalled_ThenThrowsException(decimal amount, decimal paidIn)
        {
            //Arrange
            var model = new Account
            {
                PaidIn = paidIn
            };

            //Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => model.Credit(amount),$"This should throw error for amount: {amount}, paid in: {paidIn}");
            Assert.AreEqual("Account pay in limit reached", exception.Message);
        }
        
        [TestCase(2000, 2000, 4000, true)]
        [TestCase(200, 2000, 2200, false)]
        [TestCase(200, 199, 399, false)]
        [TestCase(199, 200, 399, false)]
        [TestCase(199.99, 200, 399.99, false)]
        [TestCase(199, 200.998, 399.998, false)]
        [TestCase(0, 200, 200, false)]
        [TestCase(-200, 200, 0, false)]
        [TestCase(1, 3999, 4000, true)]
        [TestCase(3999, 1, 4000, true)]
        [TestCase(5000, -1000, 4000, true)]
        [TestCase(-1000, 5000, 4000, true)]
        [TestCase(101, 999, 1100, false)]
        [TestCase(-19999, -999, -20998, false)]
        [TestCase(-100, -100, -200, false)]
        public void GivenValidCombinations_WhenCreditMethodIsCalled_ThenDoesNotThrowException_AndReturnsExpectedResult(decimal amount, decimal paidIn, decimal expectedPayIn, bool expectedLowPayInFlag)
        {
            //Arrange
            var model = new Account
            {
                PaidIn = paidIn
            };

            //Act & Asserts
            Assert.DoesNotThrow(() => model.Credit(amount),"This should not throw error");
            Assert.AreEqual(expectedPayIn, model.PaidIn, "Does not match expected PayIn amount");
            Assert.AreEqual(amount, model.Balance, "Does not match expected Withdrawn amount");
            Assert.AreEqual(expectedLowPayInFlag, model.IsPayInLimitApproaching, "Does not match expected flag set");
        }
    }
}