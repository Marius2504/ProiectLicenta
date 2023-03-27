using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>
    {
        public TicketRepository(DataContext context) : base(context)
        {
        }
        public async Task<Ticket?> GetTicketWithIncludes(int id)
        {
            return await _context.Ticket.Include(t => t.Event).Include(t => t.Client).FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
