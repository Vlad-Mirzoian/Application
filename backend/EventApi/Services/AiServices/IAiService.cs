using EventApi.Dtos.AiDtos;

namespace EventApi.Services.AiServices
{
    public interface IAiService
    {
        Task<AiResponseDto> AskAsync(string question, Guid userId);
    }
}