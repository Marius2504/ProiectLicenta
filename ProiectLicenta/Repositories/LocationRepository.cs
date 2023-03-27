using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class LocationRepository : GenericRepository<Location>
    {
        public LocationRepository(DataContext context) : base(context)
        {
        }
        public async Task<Location?> GetLocationWithIncludes(int id)
        {
            return await _context.Locations.Include(l=>l.Event).FirstOrDefaultAsync(l =>l.Id == id);
        }
    }
}
