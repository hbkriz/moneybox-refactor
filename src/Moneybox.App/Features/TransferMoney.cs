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
            var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
            var toAccount = this.accountRepository.GetAccountById(toAccountId);
            
            if(fromAccount == null || toAccount == null)
            {
                throw new Exception($"Unable to find mentioned accounts");
            }

            try 
            {
                fromAccount.Debit(amount);
                toAccount.Credit(amount);
                this.accountRepository.Update(fromAccount);
                this.accountRepository.Update(toAccount);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unable to process due to the following: {ex}");
            }
            finally
            {
                if(fromAccount.IsLowBalanceApproaching && !string.IsNullOrEmpty(fromAccount.User?.Email))
                    this.notificationService.NotifyFundsLow(fromAccount.User?.Email);
                if(toAccount.IsPayInLimitApproaching && !string.IsNullOrEmpty(toAccount.User?.Email))
                    this.notificationService.NotifyApproachingPayInLimit(toAccount.User?.Email);
                if(string.IsNullOrEmpty(fromAccount.User?.Email) || string.IsNullOrEmpty(toAccount.User?.Email))
                    Console.WriteLine($"Unable to send notification due to no email address");
            }
        }
    }
}
