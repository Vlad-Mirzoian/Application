namespace EventApi.Dtos.EventDtos
{
    public class EventUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public bool? Visibility { get; set; }
    }
}