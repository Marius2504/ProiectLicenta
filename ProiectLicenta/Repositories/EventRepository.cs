using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class EventRepository : GenericRepository<Event>
    {
        public EventRepository(DataContext context) : base(context)
        {
        }
        public async Task<Event?> GetByIdWithIncludes(int id)
        {
            return await _context.Events.Include(e=>e.Location).Include(e=>e.Tickets).Include(e=>e.Artists).FirstOrDefaultAsync(e => e.Id == id);
        }
        
    }
}
