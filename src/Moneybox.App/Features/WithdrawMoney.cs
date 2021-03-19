using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
            
            if(fromAccount == null)
                throw new Exception($"Unable to find mentioned account");

            try 
            {
                fromAccount.Debit(amount);
                this.accountRepository.Update(fromAccount);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unable to process due to the following: {ex}");
            }
            finally
            {
                if(fromAccount.IsLowBalanceApproaching && !string.IsNullOrEmpty(fromAccount.User?.Email))
                    this.notificationService.NotifyFundsLow(fromAccount.User?.Email);
                if(string.IsNullOrEmpty(fromAccount.User?.Email))
                    Console.WriteLine($"Unable to send notification due to no email address");
            }
        }
    }
}
