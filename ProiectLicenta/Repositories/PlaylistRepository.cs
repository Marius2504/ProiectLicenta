using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class PlaylistRepository : GenericRepository<Playlist>
    {
        public PlaylistRepository(DataContext context) : base(context)
        {
        }
        public async Task<Playlist?> GetPlaylistWithIncludes(int id)
        {
            return await _context.Playlists.Include(p=>p.Clients).FirstOrDefaultAsync(p=>p.Id == id);
        }
    }
}
