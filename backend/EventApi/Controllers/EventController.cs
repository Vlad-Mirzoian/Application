using EventApi.Dtos.EventDtos;
using EventApi.Services.EventServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventApi.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventResponseDto>>> GetPublicEvents(
            [FromQuery] GetPublicEventsQueryDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null && Guid.TryParse(userIdClaim, out var parsedId)
                ? parsedId
                : (Guid?)null;

            var tagIds = new List<Guid>();
            if (!string.IsNullOrEmpty(dto.Tags))
            {
                tagIds = dto.Tags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => Guid.Parse(t))
                    .ToList();
            }

            var events = await _eventService.GetPublicEventsAsync(userId, tagIds);
            return Ok(events);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponseDto>> GetEventById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var eventDto = await _eventService.GetEventByIdAsync(id, userId != null ? Guid.Parse(userId) : null);
            return Ok(eventDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> CreateEvent([FromBody] EventCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = Guid.Parse(userIdClaim!);
            var eventDto = await _eventService.CreateEventAsync(dto, userId);
            return CreatedAtAction(nameof(GetEventById), new { id = eventDto.Id }, eventDto);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<EventResponseDto>> UpdateEvent(Guid id, [FromBody] EventUpdateDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var eventDto = await _eventService.UpdateEventAsync(id, dto, userId);
            return Ok(eventDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _eventService.DeleteEventAsync(id, userId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/join")]
        public async Task<ActionResult> JoinEvent(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _eventService.JoinEventAsync(id, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/leave")]
        public async Task<ActionResult> LeaveEvent(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _eventService.LeaveEventAsync(id, userId);
            return Ok();
        }
    }
}