using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class AlbumRepository : GenericRepository<Album>
    {
        public AlbumRepository(DataContext context) : base(context)
        {
        }
        public async Task<Album?> GetAlbumWithIncludes(int id)
        {
            return await _context.Albums.Include(a=>a.Artist).Include(a=>a.Songs).FirstOrDefaultAsync(a=>a.Id == id);
        }
    }
}
