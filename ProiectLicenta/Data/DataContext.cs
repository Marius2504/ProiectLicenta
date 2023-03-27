using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Entities;

namespace ProiectLicenta.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
         
        }
        DbSet<Album> Albums { get; set; }
        DbSet<Artist> Artists { get; set;}
        DbSet<Client> Clients { get; set; }
        DbSet<Event> Events { get; set; }
        DbSet<Genre> Genres { get; set; }
        DbSet<Location> Locations { get;set; }
        DbSet<Playlist> Playlists { get; set; }
        DbSet<Song> Songs { get; set; }
        DbSet<Ticket> Ticket { get; set; }
    }
}
