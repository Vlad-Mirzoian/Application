using EventApi.Dtos.EventDtos;
using EventApi.Dtos.TagDtos;
using EventApi.Middlewares;
using EventApi.Models;
using EventApi.Repositories.AuthRepositories;
using EventApi.Repositories.EventRepositories;
using EventApi.Repositories.TagRepositories;
using Microsoft.EntityFrameworkCore;

namespace EventApi.Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ITagRepository _tagRepository;

        public EventService(IEventRepository eventRepository, IAuthRepository authRepository, ITagRepository tagRepository)
        {
            _eventRepository = eventRepository;
            _authRepository = authRepository;
            _tagRepository = tagRepository;
        }

        public async Task<List<EventResponseDto>> GetPublicEventsAsync(Guid? userId = null, List<Guid>? tagIds = null)
        {
            var query = _eventRepository.GetPublicEventsQuery();

            if (tagIds != null && tagIds.Any())
                query = query.Where(e => e.EventTags.Any(et => tagIds.Contains(et.TagId)));

            if (userId.HasValue)
                query = query.Where(e => e.CreatorId != userId.Value);

            var events = await query.ToListAsync();

            return events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDateTime = e.StartDateTime,
                Location = e.Location,
                Capacity = e.Capacity,
                Visibility = e.Visibility,
                CreatorId = e.CreatorId,
                CreatorEmail = e.Creator.Email,
                ParticipantCount = e.Participants?.Count ?? 0,
                Tags = e.EventTags.Select(et => new TagDto
                {
                    Id = et.Tag.Id,
                    Name = et.Tag.Name
                }).ToList()
            }).ToList();
        }

        public async Task<EventResponseDto> GetEventByIdAsync(Guid id, Guid? userId)
        {
            var e = await _eventRepository.GetByIdAsync(id);
            if (e == null) throw new EventNotFoundException("Event not found");
            if (!e.Visibility && userId != e.CreatorId) throw new ForbiddenAccessException("Private event access denied");

            return new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDateTime = e.StartDateTime,
                Location = e.Location,
                Capacity = e.Capacity,
                Visibility = e.Visibility,
                CreatorId = e.CreatorId,
                CreatorEmail = e.Creator.Email,
                ParticipantCount = e.Participants.Count,
                Tags = e.EventTags.Select(et => new TagDto
                {
                    Id = et.Tag.Id,
                    Name = et.Tag.Name
                }).ToList()
            };
        }

        public async Task<EventResponseDto> CreateEventAsync(EventCreateDto dto, Guid userId)
        {
            var existingTags = await _tagRepository.GetByIdsAsync(dto.TagIds);
            if (existingTags.Count != dto.TagIds.Count)
                throw new TagsNotFoundException("One or more selected tags do not exist.");

            var e = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDateTime = dto.StartDateTime,
                Location = dto.Location,
                Capacity = dto.Capacity,
                Visibility = dto.Visibility,
                CreatorId = userId,
                CreatedAt = DateTime.UtcNow,
                EventTags = dto.TagIds.Select(tagId => new EventTag
                {
                    TagId = tagId
                }).ToList()
            };

            await _eventRepository.AddAsync(e);
            var user = await _authRepository.GetByIdAsync(userId);

            return new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDateTime = e.StartDateTime,
                Location = e.Location,
                Capacity = e.Capacity,
                Visibility = e.Visibility,
                CreatorId = e.CreatorId,
                CreatorEmail = user!.Email,
                ParticipantCount = 0,
                Tags = existingTags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            };
        }

        public async Task<EventResponseDto> UpdateEventAsync(Guid id, EventUpdateDto dto, Guid userId)
        {
            if (dto.TagIds != null)
            {
                var existingTags = await _tagRepository.GetByIdsAsync(dto.TagIds);
                if (existingTags.Count != dto.TagIds.Count)
                    throw new TagsNotFoundException("One or more selected tags do not exist.");
            }

            var e = await _eventRepository.GetByIdAsync(id);
            if (e == null) throw new EventNotFoundException("Event not found");
            if (e.CreatorId != userId) throw new ForbiddenAccessException("Only creator can edit event");

            if (dto.Title != null) e.Title = dto.Title;
            if (dto.Description != null) e.Description = dto.Description;
            if (dto.StartDateTime.HasValue) e.StartDateTime = dto.StartDateTime.Value;
            if (dto.Location != null) e.Location = dto.Location;
            if (dto.Capacity.HasValue) e.Capacity = dto.Capacity;
            if (dto.Visibility.HasValue) e.Visibility = dto.Visibility.Value;
            e.UpdatedAt = DateTime.UtcNow;

            if (dto.TagIds != null)
            {
                e.EventTags.Clear();
                foreach (var tagId in dto.TagIds)
                    e.EventTags.Add(new EventTag { TagId = tagId });
            }

            await _eventRepository.UpdateAsync(e);

            var user = e.Creator;
            var tags = e.EventTags.Select(et => et.Tag);

            return new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDateTime = e.StartDateTime,
                Location = e.Location,
                Capacity = e.Capacity,
                Visibility = e.Visibility,
                CreatorId = e.CreatorId,
                CreatorEmail = user.Email,
                ParticipantCount = e.Participants?.Count ?? 0,
                Tags = tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            };
        }

        public async Task DeleteEventAsync(Guid id, Guid userId)
        {
            var e = await _eventRepository.GetByIdAsync(id);
            if (e == null) throw new EventNotFoundException("Event not found");
            if (e.CreatorId != userId) throw new ForbiddenAccessException("Only creator can delete event");
            await _eventRepository.DeleteAsync(e);
        }

        public async Task JoinEventAsync(Guid id, Guid userId)
        {
            var @event = await _eventRepository.GetByIdAsync(id)
                ?? throw new EventNotFoundException("Event not found");

            if (!@event.Visibility && @event.CreatorId != userId)
                throw new ForbiddenAccessException("Private event access denied");

            if (@event.CreatorId == userId)
                throw new ForbiddenAccessException("Creator cannot join own event");

            if (@event.Participants.Any(p => p.UserId == userId))
                throw new AlreadyJoinedException("User already joined");

            await _eventRepository.AddParticipantAsync(id, userId);
        }

        public async Task LeaveEventAsync(Guid id, Guid userId)
        {
            var @event = await _eventRepository.GetByIdAsync(id)
                ?? throw new EventNotFoundException("Event not found");

            if (!@event.Participants.Any(p => p.UserId == userId))
                throw new NotParticipantException("User is not a participant");

            await _eventRepository.RemoveParticipantAsync(id, userId);
        }

        public async Task<List<CalendarEventDto>> GetUserEventsAsync(Guid userId)
        {
            var events = await _eventRepository.GetUserEventsAsync(userId);

            return events.Select(e => new CalendarEventDto
            {
                Id = e.Id,
                Title = e.Title,
                Start = e.StartDateTime,
                IsCreator = e.CreatorId == userId
            }).ToList();
        }
    }
}