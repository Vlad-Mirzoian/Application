using EventApi.Dtos.AiDtos;
using FluentValidation;

namespace EventApi.Validators.AiValidators
{
    public class AiRequestValidator : AbstractValidator<AiRequestDto>
    {
        public AiRequestValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Question is required")
                .MaximumLength(200).WithMessage("Question must be at most 200 characters");
        }
    }
}