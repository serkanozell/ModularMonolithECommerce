using BuildingBlocks.Shared.Dtos;

namespace Inventory.Contracts;

public record ReserveStockIntegrationEvent(
    Guid SagaCorrelationId,
    Guid OrderId,
    List<OrderItemDto> OrderItems);

public record StockReservedIntegrationEvent(
    Guid SagaCorrelationId,
    Guid OrderId);

public record StockReserveFailedIntegrationEvent(
    Guid SagaCorrelationId,
    Guid OrderId,
    string FailureReason);

public record ReleaseReserveIntegrationEvent(
    Guid SagaCorrelationId,
    Guid OrderId);
