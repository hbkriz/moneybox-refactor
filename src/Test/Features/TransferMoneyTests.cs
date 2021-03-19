using NUnit.Framework;
using Moneybox.App;
using Moneybox.App.Features;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;
using Moq;

namespace Test.Features
{
    [TestFixture]
    public class TransferMoneyTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<INotificationService> _notificationServiceMock;
        private TransferMoney _transferMoney;
        private Guid _accountId = Guid.Parse("9546482E-887A-4CAB-A403-AD9C326FFDA5");
        private Guid _userId = Guid.Parse("200adb3d-b3f4-4bde-a9c8-2c6888d6be30");
        
        [SetUp]
        public void SetUp()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _transferMoney = new TransferMoney(_accountRepositoryMock.Object, _notificationServiceMock.Object);
        }

        [Test]
        //Balance
        [TestCase(1000, 0, 200, 2, 2, 0, 0)] //Postive scenario
        [TestCase(500, 0, 500, 2, 2, 1, 0)] //Semi-Postive scenario which will notify low balance
        [TestCase(0, 0, 500, 2, 0, 0, 0)] //Negative scenario - fails updating due to Get method failing
        //PaidIn
        [TestCase(1000, 200, 200, 2, 2, 0, 0)] //Postive scenario
        [TestCase(2000, 3300, 400, 2, 2, 0, 1)] //Semi-Postive scenario which will notify low pay in limit
        [TestCase(2000, 500, 4000, 2, 0, 0, 0)] //Negative scenario - fails updating due to Get method failing
        public void Testing_AllCombinations_ForTransfer(
        decimal balance, decimal paidIn, decimal amount, 
        int get_account_count,
        int update_account_count,
        int notify_funds_low_count,
        int notify_payin_limit_count)
        {
            //Arrange
            _accountRepositoryMock.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
            .Returns(new Account 
            {
                Id =_accountId,
                PaidIn = paidIn,
                Balance = balance,
                User = new User 
                {
                    Id = _userId,
                    Name = "Test",
                    Email = "test@mail.com"
                }
            });
            
            _accountRepositoryMock.Setup(x => x.Update(It.IsAny<Account>())).Verifiable();

            _notificationServiceMock.Setup(x => x.NotifyFundsLow(It.IsAny<string>())).Verifiable();
            _notificationServiceMock.Setup(x => x.NotifyApproachingPayInLimit(It.IsAny<string>())).Verifiable();

            //Act
            _transferMoney.Execute(_accountId, _accountId, amount);

            //Assert
            _accountRepositoryMock.Verify(x => x.GetAccountById(It.IsAny<Guid>()), Times.Exactly(get_account_count));
            _accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(update_account_count));
            _notificationServiceMock.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Exactly(notify_funds_low_count));
            _notificationServiceMock.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Exactly(notify_payin_limit_count));
        }
    }
}