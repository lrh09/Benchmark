using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace ExecutionBenchmark.Connections
{
    //database context for postgre
    internal class PostgreContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Constants.PostgresConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>()
                .Property(e => e.Created).HasConversion(t => t.ToDateTime(), t => t.ToTimestamp());
            modelBuilder.Entity<Record>()
                .Property(e => e.Updated).HasConversion(t => t.ToDateTime(), t => t.ToTimestamp());
        }
    }
}
