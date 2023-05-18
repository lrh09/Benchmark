using ExecutionBenchmark.Models;
using Microsoft.EntityFrameworkCore;

namespace ExecutionBenchmark.DbContexts;

public class MsSqlDbContext : DbContext
{
    public DbSet<CryptoOrder> Orders { get; set; }
    public DbSet<TradeReport> TradeReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Constants.MsSqlConnectionString);
    }
}