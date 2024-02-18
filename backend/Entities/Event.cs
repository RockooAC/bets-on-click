using System;

namespace WebApi.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventStop { get; set; }
    }
}
