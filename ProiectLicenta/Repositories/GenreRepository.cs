using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class GenreRepository : GenericRepository<Genre>
    {
        public GenreRepository(DataContext context) : base(context)
        {
        }
        public async Task<Genre?> GetByIdWithIncludes(int id)
        {
            return await _context.Genres.Include(x => x.Songs).FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
