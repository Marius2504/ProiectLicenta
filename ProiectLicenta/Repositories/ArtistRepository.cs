using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class ArtistRepository : GenericRepository<Artist>
    {
        public ArtistRepository(DataContext context) : base(context)
        {
        }
        public async Task<Artist?> GetArtistWithIncludes(int id)
        {
            return await _context.Artists.Include(a=>a.Albums).Include(a=>a.Songs).FirstOrDefaultAsync(a=>a.Id == id);
        }

        public async Task<Artist> GetArtistOfUser(string userId)
        {
            return await _context.Artists.Where(a => a.AppUserId == userId).FirstOrDefaultAsync();
        }
    }
}
