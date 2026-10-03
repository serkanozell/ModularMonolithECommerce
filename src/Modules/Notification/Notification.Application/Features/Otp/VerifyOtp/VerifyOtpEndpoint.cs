namespace Notification.Application.Features.Otp.VerifyOtp
{
    public record VerifyOtpRequest(string Recipient, string Purpose, string Code);

    public record VerifyOtpResponse(bool IsValid);

    public class VerifyOtpEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/notifications/otp/verify", async (VerifyOtpRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new VerifyOtpCommand(request.Recipient, request.Purpose, request.Code), cancellationToken);

                return Results.Ok(new VerifyOtpResponse(result.IsValid));
            })
            .WithName("VerifyOtp")
            .WithTags("Notifications")
            .Produces<VerifyOtpResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}