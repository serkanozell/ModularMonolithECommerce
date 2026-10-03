namespace Notification.Application.Features.Otp.SendOtp
{
    public record SendOtpRequest(string Recipient, OtpChannel Channel, string Purpose);

    public record SendOtpResponse(Guid OtpId, DateTime ExpiresAt);

    public class SendOtpEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/notifications/otp/send", async (SendOtpRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new SendOtpCommand(request.Recipient, request.Channel, request.Purpose), cancellationToken);

                return Results.Ok(new SendOtpResponse(result.OtpId, result.ExpiresAt));
            })
            .WithName("SendOtp")
            .WithTags("Notifications")
            .Produces<SendOtpResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}