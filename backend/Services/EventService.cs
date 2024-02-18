using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class EventService
    {
        private readonly DataContext _context;

        public EventService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEventById(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<Event> CreateEvent(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<bool> UpdateEvent(Event eventEntity)
        {
            var existingEvent = await _context.Events.FindAsync(eventEntity.Id);
            if (existingEvent == null)
            {
                return false;
            }

            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return false;
            }

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
