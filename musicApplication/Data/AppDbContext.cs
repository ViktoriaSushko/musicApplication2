using Microsoft.EntityFrameworkCore;
using musicApplication.Model;

namespace musicApplication.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Song> Songs => Set<Song>();

    }
}
