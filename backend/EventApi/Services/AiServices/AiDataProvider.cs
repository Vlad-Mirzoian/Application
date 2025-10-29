using EventApi.Dtos.AiDtos;
using EventApi.Dtos.EventDtos;
using EventApi.Repositories.EventRepositories;
using EventApi.Repositories.TagRepositories;
using Microsoft.EntityFrameworkCore;

namespace EventApi.Services.AiServices
{
    public class AiDataProvider : IAiDataProvider
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITagRepository _tagRepository;

        public AiDataProvider(IEventRepository eventRepo, ITagRepository tagRepo)
        {
            _eventRepository = eventRepo;
            _tagRepository = tagRepo;
        }

        public async Task<AiContextDto> GetContextAsync(Guid userId, string question)
        {
            var context = new AiContextDto();

            var userEvents = await _eventRepository.GetUserEventsAsync(userId);
            context.UserEvents = userEvents.Select(e => new UserEventDto
            {
                Id = e.Id,
                Title = e.Title,
                Start = e.StartDateTime,
                Role = e.CreatorId == userId ? "organizer" : "participant",
                Location = e.Location,
                Tags = e.EventTags.Select(et => et.Tag.Name).ToList(),
                ParticipantNames = e.Participants.Select(p => p.User.Email).ToList()
            }).ToList();

            var allTags = await _tagRepository.GetAllAsync();

            var matchedTags = allTags
                .Where(t => question.Contains(t.Name, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Id)
                .ToList();

            if (matchedTags.Any())
            {
                var publicEvents = await _eventRepository.GetPublicEventsQuery()
                    .Where(e => e.EventTags.Any(et => matchedTags.Contains(et.TagId)))
                    .OrderBy(e => e.StartDateTime)
                    .ToListAsync();

                context.PublicEvents = publicEvents.Select(e => new PublicEventDto
                {
                    Title = e.Title,
                    Start = e.StartDateTime,
                    Tags = e.EventTags.Select(et => et.Tag.Name).ToList(),
                    Location = e.Location,
                    ParticipantNames = e.Participants.Select(p => p.User.Email).ToList() // <-- сюда участников
                }).ToList();
            }

            return context;
        }
    }
}