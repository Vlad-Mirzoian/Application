using EventApi.Dtos.EventDtos;

namespace EventApi.Services.EventServices
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetPublicEventsAsync(Guid? userId = null, List<Guid>? tagIds = null);
        Task<EventResponseDto> GetEventByIdAsync(Guid id, Guid? userId);
        Task<EventResponseDto> CreateEventAsync(EventCreateDto dto, Guid userId);
        Task<EventResponseDto> UpdateEventAsync(Guid id, EventUpdateDto dto, Guid userId);
        Task DeleteEventAsync(Guid id, Guid userId);
        Task JoinEventAsync(Guid id, Guid userId);
        Task LeaveEventAsync(Guid id, Guid userId);
        Task<List<CalendarEventDto>> GetUserEventsAsync(Guid userId);
    }
}