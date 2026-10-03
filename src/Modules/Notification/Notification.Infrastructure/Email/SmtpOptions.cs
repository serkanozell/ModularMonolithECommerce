using System.ComponentModel.DataAnnotations;

namespace Notification.Infrastructure.Email
{
    public sealed class SmtpOptions
    {
        public const string SectionName = "Notification:Smtp";

        [Required]
        public string Host { get; set; } = default!;

        [Range(1, 65535)]
        public int Port { get; set; } = 587;

        public bool EnableSsl { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        [Required, EmailAddress]
        public string FromAddress { get; set; } = default!;

        public string FromName { get; set; } = "ModularMonolithECommerce";
    }
}
