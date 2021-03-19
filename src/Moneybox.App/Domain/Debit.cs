using System;

namespace Moneybox.App
{
    public partial class Account
    {
        public bool IsLowBalanceApproaching { get; set; }

        public void Debit(decimal amount)
        {
            if (Balance - amount < 0)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            Balance -= amount;
            Withdrawn -= amount;

            SetLowBalanceApproaching();
        }

        private void SetLowBalanceApproaching()
        {
            if (Balance < MinimalLimit)
            {
                IsLowBalanceApproaching = true;
            }
        }
    }
}