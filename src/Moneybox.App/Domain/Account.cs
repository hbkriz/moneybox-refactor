using System;

namespace Moneybox.App
{
    public class Account
    {
        private const decimal PayInLimit = 4000m;

        private const decimal MinimalLimit = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public bool IsLowBalanceApproaching { get; set; }

        public bool IsPayInLimitApproaching { get; set; }

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

        public void Credit(decimal amount)
        {
            if (PaidIn + amount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            Balance += amount;
            PaidIn += amount;
            
            SetPayInLimitApproaching();
        }

        public void SetLowBalanceApproaching()
        {
            if (Balance < MinimalLimit)
            {
                IsLowBalanceApproaching = true;
            }
        }

        public void SetPayInLimitApproaching()
        {
            if (PayInLimit - PaidIn < MinimalLimit)
            {
                IsPayInLimitApproaching = true;
            }
        }
    }
}
