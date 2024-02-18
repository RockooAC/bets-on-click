using System;

namespace WebApi.Models.Events
{
    public class CreateEventRequest
    {
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventStop { get; set; }
    }
}
