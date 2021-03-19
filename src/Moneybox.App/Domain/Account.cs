using System;

namespace Moneybox.App
{
    public partial class Account
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }
        
        private const decimal PayInLimit = 4000m;

        private const decimal MinimalLimit = 500m;
    }
}
