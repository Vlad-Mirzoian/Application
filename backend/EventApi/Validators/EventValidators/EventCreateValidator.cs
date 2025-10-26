using EventApi.Dtos.EventDtos;
using FluentValidation;

namespace EventApi.Validators.EventValidators
{
    public class EventCreateValidator : AbstractValidator<EventCreateDto>
    {
        public EventCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title must be at most 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must be at most 1000 characters");

            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Event start must be in the future");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location must be at most 200 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).When(x => x.Capacity.HasValue)
                .WithMessage("Capacity must be greater than zero if specified");
        }
    }
}