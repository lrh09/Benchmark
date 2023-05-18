using ExecutionBenchmark.Models;
using Microsoft.EntityFrameworkCore;

namespace ExecutionBenchmark.DbContexts;

public class PostgresDbContext : DbContext
{
    public DbSet<CryptoOrder> CryptoOrders { get; set; }
    public DbSet<TradeReport> TradeReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Constants.PostgresConnectionString);
    }
}