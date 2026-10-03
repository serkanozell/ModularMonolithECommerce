namespace BuildingBlocks.Messaging.Events
{
    public record SendOtpIntegrationEvent(Guid Id,
                                          string Recipient,
                                          string Purpose,
                                          string Text) : IntegrationEvent;
}