using EventApi.Dtos.EventDtos;
using EventApi.Services.EventServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IEventService _eventService;

        public UserController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [Authorize]
        [HttpGet("me/events")]
        public async Task<ActionResult<List<CalendarEventDto>>> GetUserEvents()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var events = await _eventService.GetUserEventsAsync(userId);
            return Ok(events);
        }
    }
}