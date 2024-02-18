using System;

namespace WebApi.Models.Events
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventStop { get; set; }
    }
}
