using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Models.Events;
using WebApi.Services;
using System;


namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<EventsController> _logger;

        public EventsController(
            EventService eventService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var @event = await _eventService.GetEventById(id);
            if (@event == null)
            {
                _logger.LogInformation($"Event with id {id} not found.");
                return NotFound(new { message = "Event not found" });
            }

            return Ok(@event);
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest model)
        {
            var eventEntity = new Event
            {
                EventName = model.EventName,
                EventStart = model.EventStart,
                EventStop = model.EventStop
            };

            await _eventService.CreateEvent(eventEntity);

            _logger.LogInformation($"Event created: {eventEntity.Id}");
            return CreatedAtAction(nameof(GetEventById), new { id = eventEntity.Id }, eventEntity);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest model)
        {

            var eventEntity = await _eventService.GetEventById(id);
            if (eventEntity == null)
            {
                _logger.LogInformation($"Event with id {id} not found.");
                return NotFound(new { message = "Event not found" });
            }


            eventEntity.EventName = model.EventName;
            eventEntity.EventStart = model.EventStart;
            eventEntity.EventStop = model.EventStop;


            try
            {
                await _eventService.UpdateEvent(eventEntity);
                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating event with id {id}.");
                return StatusCode(500, new { message = "Error updating the event" });
            }
        }


        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var success = await _eventService.DeleteEvent(id);
            if (!success)
            {
                _logger.LogInformation($"Event with id {id} not found for deletion.");
                return NotFound(new { message = "Event not found" });
            }

            return Ok(new { message = "Event deleted successfully" });
        }
    }
}
