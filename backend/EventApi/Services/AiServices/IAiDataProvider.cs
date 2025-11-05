using EventApi.Dtos.AiDtos;

namespace EventApi.Services.AiServices
{
    public interface IAiDataProvider
    {
        Task<AiContextDto> GetContextAsync(Guid userId, string question);
    }
}