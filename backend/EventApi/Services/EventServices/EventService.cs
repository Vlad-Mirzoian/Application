using EventApi.Dtos.EventDtos;
using EventApi.Middlewares;
using EventApi.Models;
using EventApi.Repositories.AuthRepositories;
using EventApi.Repositories.EventRepositories;

namespace EventApi.Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAuthRepository _authRepository;

        public EventService(IEventRepository eventRepository, IAuthRepository authRepository)
        {
            _eventRepository = eventRepository;
            _authRepository = authRepository;
        }

        public async Task<List<EventResponseDto>> GetPublicEventsAsync(Guid? userId = null)
        {
            var events = await _eventRepository.GetPublicEventsAsync();

            if (userId.HasValue)
            {
                events = events.Where(e => e.CreatorId != userId.Value).ToList();
            }

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
                ParticipantCount = e.Participants?.Count ?? 0
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
                ParticipantCount = e.Participants.Count
            };
        }

        public async Task<EventResponseDto> CreateEventAsync(EventCreateDto dto, Guid userId)
        {
            var e = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDateTime = dto.StartDateTime,
                Location = dto.Location,
                Capacity = dto.Capacity,
                Visibility = dto.Visibility,
                CreatorId = userId,
                CreatedAt = DateTime.UtcNow
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
                ParticipantCount = 0
            };
        }

        public async Task<EventResponseDto> UpdateEventAsync(Guid id, EventUpdateDto dto, Guid userId)
        {
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

            await _eventRepository.UpdateAsync(e);
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
                ParticipantCount = e.Participants?.Count ?? 0
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
            return await _eventRepository.GetUserEventsAsync(userId);
        }
    }
}