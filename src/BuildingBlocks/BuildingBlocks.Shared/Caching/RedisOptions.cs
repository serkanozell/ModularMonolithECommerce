using FluentValidation;

namespace BuildingBlocks.Shared.Caching
{
    public class RedisOptions
    {
        public int DefaultAbsoluteExpirationInMinutes { get; set; } = 60;
        public int DefaultSlidingExpirationInMinutes { get; set; } = 30;
    }

    //Options Pattern Validation eklenecek
    public class RedisOptionsValidator : AbstractValidator<RedisOptions>
    {
        public RedisOptionsValidator()
        {
            RuleFor(x => x.DefaultAbsoluteExpirationInMinutes).GreaterThan(0)
                                                              .WithMessage("DefaultAbsoluteExpirationInMinutes must be greater than 0.");

            RuleFor(x => x.DefaultSlidingExpirationInMinutes).GreaterThan(0)
                                                             .WithMessage("DefaultSlidingExpirationInMinutes must be greater than 0.");
        }
    }
}