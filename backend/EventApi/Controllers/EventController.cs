using EventApi.Dtos.EventDtos;
using EventApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventApi.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventResponseDto>>> GetPublicEvents()
        {
            var events = await _eventService.GetPublicEventsAsync();
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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
    }
}