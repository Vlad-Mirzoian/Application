using EventApi.Dtos.EventDtos;
using FluentValidation;

namespace EventApi.Validators.EventValidators
{
    public class EventUpdateValidator : AbstractValidator<EventUpdateDto>
    {
        public EventUpdateValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100)
                .WithMessage("Title must be at most 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must be at most 1000 characters");

            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.StartDateTime.HasValue)
                .WithMessage("Event start must be in the future if specified");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .When(x => x.Location != null)
                .WithMessage("Location must be at most 200 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .When(x => x.Capacity.HasValue)
                .WithMessage("Capacity must be greater than zero if specified");
        }
    }
}