using EventApi.Dtos.EventDtos;
using FluentValidation;

public class GetPublicEventsQueryValidator : AbstractValidator<GetPublicEventsQueryDto>
{
    public GetPublicEventsQueryValidator()
    {
        RuleFor(x => x.Tags)
            .Must(tags =>
            {
                if (string.IsNullOrEmpty(tags)) return true;
                return tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .All(t => Guid.TryParse(t, out _));
            })
            .WithMessage("One or more tags are invalid GUIDs.");
    }
}
