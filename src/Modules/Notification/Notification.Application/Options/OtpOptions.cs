using System.ComponentModel.DataAnnotations;

namespace Notification.Application.Options
{
    public sealed class OtpOptions
    {
        public const string SectionName = "Notification:Otp";

        [Range(4, 10)]
        public int CodeLength { get; set; } = 6;

        [Range(1, 60)]
        public int LifetimeInMinutes { get; set; } = 3;

        [Range(1, 20)]
        public int MaxAttempts { get; set; } = 5;

        [Range(1, 100)]
        public int MaxRequestsPerHour { get; set; } = 5;
    }
}
