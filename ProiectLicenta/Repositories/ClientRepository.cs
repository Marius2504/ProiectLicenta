using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class ClientRepository : GenericRepository<Client>
    {
        public ClientRepository(DataContext context) : base(context)
        {
        }
        public async Task<Client?> GetClientWithIncludes(int id)
        {
            return await _context.Clients.Include(c=>c.Ticket).Include(c=>c.Playlists).FirstOrDefaultAsync(c=>c.Id== id);
        }
    }
}
