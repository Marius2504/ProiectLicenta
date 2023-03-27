using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
         
        }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set;}
        public DbSet<Client> Clients { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Location> Locations { get;set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
    }
}
