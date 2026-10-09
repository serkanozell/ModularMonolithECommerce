namespace Notification.Domain.ValueObjects
{
    public record OtpCodeId
    {
        public Guid Value { get; }

        private OtpCodeId(Guid value) => Value = value;

        public static OtpCodeId Of(Guid value)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty);

            return new OtpCodeId(value);
        }
    }
}