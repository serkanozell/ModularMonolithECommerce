namespace Payment.Contracts.PaymentIntegrationEvents;

public record ProcessPaymentIntegrationEvent(Guid SagaCorrelationId, Guid OrderId, Guid CustomerId, decimal Amount);
public record PaymentSucceededIntegrationEvent(Guid SagaCorrelationId, Guid OrderId);
public record PaymentFailedIntegrationEvent(Guid SagaCorrelationId, Guid OrderId, string FailureReason);
public record RefundPaymentIntegrationEvent(Guid SagaCorrelationId, Guid OrderId);
public record PaymentRefundResultIntegrationEvent(Guid SagaCorrelationId, Guid OrderId, bool IsSuccess, string? FailureReason = null);