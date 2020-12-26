using Microsoft.EntityFrameworkCore;

namespace CSBelt.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options){}
        public DbSet<Users> Users{get;set;}
        public DbSet<Events> Events{get;set;}
        public DbSet<Participants> Participants{get;set;}
    }
}