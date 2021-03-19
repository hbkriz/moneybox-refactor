using System;

namespace Moneybox.App
{
    public partial class Account
    {
        public bool IsPayInLimitApproaching { get; set; }

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

        private void SetPayInLimitApproaching()
        {
            if (PayInLimit - PaidIn < MinimalLimit)
            {
                IsPayInLimitApproaching = true;
            }
        }
    }
}