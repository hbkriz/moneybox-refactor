using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            var fromBalance = from.Balance - amount;
            if (fromBalance < 0m)
            {
                this.notificationService.NotifyInsufficientFunds(from.User.Email);
                return;
            }

            if (fromBalance < 500m)
            {
                this.notificationService.NotifyFundsLow(from.User.Email);
            }

            var paidIn = to.PaidIn + amount;
            if (paidIn > Account.PayInLimit)
            {
                this.notificationService.NotifyPayInLimitReached(to.User.Email);
                return;
            }

            if (Account.PayInLimit - paidIn < 500m)
            {
                this.notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }

            from.Balance = from.Balance - amount;
            from.Withdrawn = from.Withdrawn - amount;

            to.Balance = to.Balance + amount;
            to.PaidIn = to.PaidIn + amount;

            this.accountRepository.Update(from);
            this.accountRepository.Update(to);
        }
    }
}
