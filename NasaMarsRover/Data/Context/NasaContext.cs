using Business.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class NasaContext : DbContext
    {
        public NasaContext() { }
        public NasaContext(DbContextOptions<NasaContext> options) : base(options) { }

        public DbSet<Rover> Rovers { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
