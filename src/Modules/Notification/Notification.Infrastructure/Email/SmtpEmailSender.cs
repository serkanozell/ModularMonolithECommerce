using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Abstractions;
using System.Net;
using System.Net.Mail;

namespace Notification.Infrastructure.Email
{
    public sealed class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
    {
        private readonly SmtpOptions _options = options.Value;

        public async Task SendAsync(string to, string subject, string body, bool isHtml, CancellationToken cancellationToken = default)
        {
            var message = new MailMessage(_options.FromAddress,
                                          to,
                                          subject,
                                          body)
            {
                IsBodyHtml = isHtml
            };

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(
                    _options.UserName,
                    _options.Password),
                EnableSsl = _options.EnableSsl
            };

            await client.SendMailAsync(message, cancellationToken);

            logger.LogInformation("Email sent to {To} with subject {Subject}", message.To, message.Subject);
        }
    }
}