using EventApi.Dtos.EventDtos;

namespace EventApi.Dtos.AiDtos
{
    public class AiContextDto
    {
        public DateTime CurrentTime { get; set; } = DateTime.UtcNow;
        public List<UserEventDto> UserEvents { get; set; } = new();
        public List<PublicEventDto> PublicEvents { get; set; } = new();
    }
}