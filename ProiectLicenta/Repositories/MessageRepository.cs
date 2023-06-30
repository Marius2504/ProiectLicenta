using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository(DataContext context) : base(context)
        {
        }
        public async Task<Message?> GetByIdWithIncludes(int id)
        {
            return await _context.Messages.Include(m =>m.Song).Include(m=>m.LikesFromUsers).FirstOrDefaultAsync(e => e.Id == id);
        }

    }
}
