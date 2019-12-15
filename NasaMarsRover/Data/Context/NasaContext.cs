using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Context
{
    public class NasaContext : DbContext
    {
        public DbSet<Rover> Rovers { get; set; }
        public DbSet<Camera> Camera { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
