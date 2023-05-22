using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class SongRepository : GenericRepository<Song>
    {
        public SongRepository(DataContext context) : base(context)
        {
        }
        public async Task<Song?> GetSongWithIncludes(int id)
        {
            return await _context.Songs.Include(s=>s.Album).Include(s=>s.Genre).FirstOrDefaultAsync(s=>s.Id == id);
        }
        public IQueryable<Song> GetAllQuerry()
        {
            return _context.Set<Song>().AsQueryable();
        }
    }
}
