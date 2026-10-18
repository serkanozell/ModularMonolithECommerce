using Payment.Domain.Enums;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities
{
    public class PaymentTransaction : Aggregate<PaymentTransactionId>
    {
        public OrderId OrderId { get; private set; } = default!;
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? FailureReason { get; private set; }

        private PaymentTransaction() { }

        private PaymentTransaction(OrderId orderId, decimal amount)
        {
            Id = PaymentTransactionId.Of(Guid.NewGuid());
            OrderId = orderId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            IsActive = true;
            IsDeleted = false;
        }

        public static PaymentTransaction Create(OrderId orderId, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(orderId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            return new PaymentTransaction(orderId, amount);
        }

        public void MarkSucceeded()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Only pending payments can succeed. Current: {Status}");
            Status = PaymentStatus.Succeeded;
        }

        public void MarkFailed(string reason)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reason);
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Only pending payments can fail. Current: {Status}");
            Status = PaymentStatus.Failed;
            FailureReason = reason;
        }

        // Idempotent: sadece Succeeded ise Refunded olur, diğer durumlarda no-op.
        public void Refund()
        {
            if (Status == PaymentStatus.Succeeded)
                Status = PaymentStatus.Refunded;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;
        }
    }
}