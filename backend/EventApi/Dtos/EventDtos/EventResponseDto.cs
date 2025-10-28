namespace EventApi.Dtos.EventDtos
{
    public class EventResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public bool Visibility { get; set; }
        public List<Guid> TagIds { get; set; } = new();
        public Guid CreatorId { get; set; }
        public string CreatorEmail { get; set; } = string.Empty;
        public int ParticipantCount { get; set; }
    }
}