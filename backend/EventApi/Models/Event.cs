using System;
using System.Collections.Generic;

namespace EventApi.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public bool Visibility { get; set; } = true;
        public Guid CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public User Creator { get; set; } = null!;
        public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    }
}