using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Options;
using Notification.Application.Options;
using Notification.Application.Services;

namespace Notification.Application.Features.Otp.SendOtp
{
    public record SendOtpCommand(string Recipient, OtpChannel Channel, string Purpose) : ICommand<SendOtpResult>;

    public record SendOtpResult(Guid OtpId, DateTime ExpiresAt);

    public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
    {
        public SendOtpCommandValidator()
        {
            RuleFor(x => x.Recipient)
                .NotEmpty().WithMessage("Recipient is required.")
                .MaximumLength(256);

            RuleFor(x => x.Recipient)
                .EmailAddress().WithMessage("Recipient must be a valid email address.")
                .When(x => x.Channel == OtpChannel.Email);

            RuleFor(x => x.Recipient)
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Recipient must be a valid phone number.")
                .When(x => x.Channel == OtpChannel.Sms);

            RuleFor(x => x.Channel)
                .IsInEnum().WithMessage("Channel is invalid.");

            RuleFor(x => x.Purpose)
                .NotEmpty().WithMessage("Purpose is required.")
                .MaximumLength(64);
        }
    }

    internal sealed class SendOtpCommandHandler(IOtpCodeRepository repository,
                                       IPublishEndpoint publishEndpoint,
                                       IOptions<OtpOptions> options) : ICommandHandler<SendOtpCommand, SendOtpResult>
    {
        private readonly OtpOptions _options = options.Value;

        public async Task<SendOtpResult> Handle(SendOtpCommand command, CancellationToken cancellationToken)
        {
            var requestCount = await repository.CountCreatedSinceAsync(command.Recipient, DateTime.UtcNow.AddHours(-1), cancellationToken);

            if (requestCount >= _options.MaxRequestsPerHour)
                throw new BadRequestException("Too many OTP requests. Please try again later.");

            var activeCodes = await repository.GetActiveAsync(command.Recipient, command.Purpose, cancellationToken);

            foreach (var activeCode in activeCodes)
                activeCode.Invalidate();

            var code = OtpCodeGenerator.Generate(_options.CodeLength);
            var channel = (NotificationChannel)command.Channel;

            var otpCode = OtpCode.Create(command.Recipient,
                                         channel,
                                         command.Purpose,
                                         OtpCodeGenerator.Hash(code),
                                         TimeSpan.FromMinutes(_options.LifetimeInMinutes),
                                         _options.MaxAttempts);

            repository.Add(otpCode);

            await repository.SaveChangesAsync(cancellationToken);

            var text = $"Doğrulama kodunuz: {code}. Kod {_options.LifetimeInMinutes} dakika geçerlidir.";

            switch (channel)
            {
                case NotificationChannel.Email:
                    await publishEndpoint.Publish(new SendOtpIntegrationEvent(otpCode.Id,
                                                                      command.Recipient,
                                                                      command.Purpose,
                                                                      text), cancellationToken);
                    break;
                    //case NotificationChannel.Sms:
                    //    await smsSender.SendAsync(new SmsMessage(command.Recipient, text), cancellationToken);
                    //    break;
            }

            return new SendOtpResult(otpCode.Id, otpCode.ExpiresAt);
        }
    }
}
