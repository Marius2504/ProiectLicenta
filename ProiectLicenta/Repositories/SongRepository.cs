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
            return await _context.Songs
                .Include(s=>s.Album)
                .Include(s=>s.Genre)
                .Include(a =>a.Artist)
                .Include(m=>m.Message)
                .Include(l =>l.UsersWhoLiked)
                .FirstOrDefaultAsync(s=>s.Id == id);
        }
        
        public async Task<Song?> GetByName(string name)
        {
            return await _context.Songs.FirstOrDefaultAsync(s=>s.Name == name);
        }
    }
}
