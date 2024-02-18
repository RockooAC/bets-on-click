using System;

namespace WebApi.Models.Events
{
    public class UpdateEventRequest
    {
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventStop { get; set; }
    }
}
