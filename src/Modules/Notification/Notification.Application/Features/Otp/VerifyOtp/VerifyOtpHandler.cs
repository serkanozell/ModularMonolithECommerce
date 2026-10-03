using Notification.Application.Services;

namespace Notification.Application.Features.Otp.VerifyOtp
{
    public record VerifyOtpCommand(string Recipient, string Purpose, string Code) : ICommand<VerifyOtpResult>;

    public record VerifyOtpResult(bool IsValid);

    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.Recipient).NotEmpty().WithMessage("Recipient is required.");
            RuleFor(x => x.Purpose).NotEmpty().WithMessage("Purpose is required.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.");
        }
    }

    internal sealed class VerifyOtpCommandHandler(IOtpCodeRepository repository) : ICommandHandler<VerifyOtpCommand, VerifyOtpResult>
    {
        public async Task<VerifyOtpResult> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            var otpCode = await repository.GetLatestActiveAsync(command.Recipient, command.Purpose, cancellationToken);

            if (otpCode is null)
                return new VerifyOtpResult(false);

            var isValid = otpCode.Verify(OtpCodeGenerator.Hash(command.Code));

            await repository.SaveChangesAsync(cancellationToken);

            return new VerifyOtpResult(isValid);
        }
    }
}
