namespace EventApi.Dtos.EventDtos
{
    public class EventCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public bool Visibility { get; set; } = true;
    }
}