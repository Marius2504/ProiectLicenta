using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class AlbumRepository : GenericRepository<Album>
    {
        private readonly SongRepository _songRepository;
        public AlbumRepository(DataContext context, SongRepository songRepository) : base(context)
        {
            this._songRepository= songRepository;
        }
        public async Task<Album?> GetAlbumWithIncludes(int id)
        {
            return await _context.Albums.Include(a=>a.Artist).Include(a=>a.Songs).FirstOrDefaultAsync(a=>a.Id == id);
        }
        public async Task<Album?> GetByName(string name)
        {
            return await _context.Albums.FirstOrDefaultAsync(s => s.Name == name);
        }
        public async Task<Album?> Delete(int id)
        {
            var obj = await GetAlbumWithIncludes(id);
            foreach(var song in obj.Songs.ToList())
            {
                await _songRepository.Delete(song.Id);
            }
            obj = await Update(obj);
            if (obj != null)
            {
                _context.Set<Album>().Remove(obj);
                await Save();
            }
            return obj;
        }
    }
}
