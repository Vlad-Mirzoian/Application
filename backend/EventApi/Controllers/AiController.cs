using System.Security.Claims;
using EventApi.Dtos.AiDtos;
using EventApi.Services.AiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventApi.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("assist")]
        [Authorize]
        public async Task<ActionResult<AiResponseDto>> Assist([FromBody] AiRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _aiService.AskAsync(dto.Question, userId);
            return Ok(result);
        }
    }
}