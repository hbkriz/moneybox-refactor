namespace Moneybox.App.Domain.Services
{
    public interface INotificationService
    {
        void NotifyApproachingPayInLimit(string emailAddress);

        void NotifyPayInLimitReached(string emailAddress);

        void NotifyInsufficientFunds(string emailAddress);

        void NotifyFundsLow(string emailAddress);
    }
}
