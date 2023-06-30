using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Repositories
{
    public class SongRepository : GenericRepository<Song>
    {
        private readonly MessageRepository _messageRepository;
        public SongRepository(DataContext context, MessageRepository messageRepository) : base(context)
        {
            this._messageRepository = messageRepository;
        }
        public async Task<Song?> GetSongWithIncludes(int id)
        {
            
            return await _context.Songs
                .Include(s=>s.Album)
                .Include(s=>s.Genre)
                .Include(a =>a.Artist)
                .Include(m=>m.Messages)
                .Include(l =>l.UsersWhoLiked)
                .FirstOrDefaultAsync(s=>s.Id == id);
        }
        
        public async Task<Song?> GetByName(string name)
        {
            return await _context.Songs.FirstOrDefaultAsync(s=>s.Name == name);
        }

        public async Task<List<Song>?> GetTrending(int start, int cantity)
        {
            return await _context.Songs.Include(s=>s.UsersWhoLiked).OrderByDescending(s => s.UsersWhoLiked.Count()).Skip(start).Take(cantity).ToListAsync();
        }
        public async Task<Song?> Delete(int id)
        {
            var obj = await GetSongWithIncludes(id);

            obj.UsersWhoLiked.Clear();

            //stergem toate mesajele
            foreach (var msg in obj.Messages.ToList())
            {
                var actualMsg = await _messageRepository.GetByIdWithIncludes(msg.Id);
                actualMsg.LikesFromUsers.Clear();
                await _messageRepository.Update(actualMsg);
                await _messageRepository.Delete(msg.Id);
            }

            await Update(obj);
            obj = await _context.Set<Song>().FindAsync(id);
            if (obj != null)
            {
                _context.Set<Song>().Remove(obj);
                await Save();
            }
            return obj;
        }
    }
}
