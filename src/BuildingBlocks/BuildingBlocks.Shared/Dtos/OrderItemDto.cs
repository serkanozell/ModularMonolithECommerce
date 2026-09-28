namespace BuildingBlocks.Shared.Dtos
{
    public record OrderItemDto(Guid ProductId, int Quantity, decimal Price);
}