using NUnit.Framework;
using Moneybox.App;
using System;

namespace Test.Domain
{
    [TestFixture]
    public class DebitMethodTests
    {
        [TestCase(234567, 500)]
        [TestCase(123456, 1000)]
        [TestCase(1235, 1234)]
        [TestCase(1235, -1234)]
        [TestCase(234567, -500)]
        [TestCase(123456, -1000)]
        public void GivenInvalidCombinations_WhenDebitMethodIsCalled_ThenThrowsException(decimal amount, decimal balance)
        {
            //Arrange
            var model = new Account
            {
                Balance = balance
            };

            //Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => model.Debit(amount),"This should throw error");
            Assert.AreEqual("Insufficient funds to make transfer", exception.Message);
        }

        [TestCase(500, 234567, 234067, false)]
        [TestCase(1000, 123456, 122456, false)]
        [TestCase(1234, 1235, 1, true)]
        [TestCase(1234, 1634, 400, true)]
        [TestCase(1000, 1500, 500, false)]
        [TestCase(1000, 1499.99, 499.99, true)]
        [TestCase(1000, 1499.998, 499.998, true)]
        [TestCase(-1235, 1234, 2469, false)]
        [TestCase(-234567, -500, 234067, false)]
        [TestCase(-123456, -1000, 122456, false)]
        public void GivenValidCombinations_WhenDebitMethodIsCalled_ThenDoesNotThrowException_AndReturnsExpectedResult(decimal amount, decimal balance, decimal expectedBalance, bool expectedLowBalanceFlag)
        {
            //Arrange
            var model = new Account
            {
                Balance = balance
            };

            //Act & Assert
            Assert.DoesNotThrow(() => model.Debit(amount),"This should not throw error");
            Assert.AreEqual(expectedBalance, model.Balance, "Does not match expected Balance amount");
            Assert.AreEqual(-amount, model.Withdrawn, "Does not match expected Withdrawn amount");
            Assert.AreEqual(expectedLowBalanceFlag, model.IsLowBalanceApproaching, "Does not match expected flag set");
        }
    }
}