using System.Data;
using System.Data.Common;
using ExecutionBenchmark.Connections;
using ExecutionBenchmark.DbContexts;
using ExecutionBenchmark.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutionBenchmark.Services;

public class RawSqlDbService
{
    private readonly DbConnection _connection;
    private readonly PostgresDbContext _context = new PostgresDbContext();

    public RawSqlDbService(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public async Task SaveOrderAndReportAsync(CryptoOrder order, TradeReport report)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }

        await using var transaction = await _connection.BeginTransactionAsync();
        try
        {
            await SaveOrderCommandAsync(order);
            await SaveReportCommandAsync(report);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SaveReportCommandAsync(TradeReport report)
    {
        string executionTime = report.ExecutionTime.ToString("yyyy-MM-dd HH:mm:ss");

        var reportSql =
            $"INSERT INTO \"TradeReports\" (\"Id\", \"OrderId\", \"Symbol\", \"ExecutedPrice\", \"ExecutedQuantity\", \"ExecutionTime\", \"ClientName\", \"Status\") VALUES ('{report.Id}', '{report.OrderId}', '{report.Symbol}', {report.ExecutedPrice}, {report.ExecutedQuantity}, '{executionTime}', '{report.ClientName}', {(int)report.Status})";

        using var reportCommand = new NpgsqlCommand(reportSql, (NpgsqlConnection)_connection); // or SqlCommand for MSSQL
        await reportCommand.ExecuteNonQueryAsync();

        // Console.WriteLine($"Saved Report: {report.Id}");
    }
    
    public async Task SaveOrderCommandAsync(CryptoOrder order)
    {
        string orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");

        var orderSql =
            $"INSERT INTO \"CryptoOrders\" (\"Id\", \"OrderId\", \"Symbol\", \"Price\", \"Quantity\", \"OrderDate\", \"Status\", \"ClientName\", \"Type\", \"StopLoss\", \"TakeProfit\") VALUES ('{order.Id}', '{order.OrderId}', '{order.Symbol}', {order.Price}, {order.Quantity}, '{orderDate}', {(int)order.Status}, '{order.ClientName}', {(int)order.Type}, {order.StopLoss}, {order.TakeProfit})";

        using var orderCommand = new NpgsqlCommand(orderSql, (NpgsqlConnection)_connection); // or SqlCommand for MSSQL
        await orderCommand.ExecuteNonQueryAsync();
        
        // Console.WriteLine($"Saved Order: {order.Id}");
    }
    
    public async Task SaveRecordAsync(Record record)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "INSERT INTO public.\"Records\" (\"Uuid\", \"Name\", \"Data0\", \"Data1\", \"Data2\", \"Data3\", \"Created\", \"Updated\") " +
            "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});",
            record.Uuid,
            record.Name,
            record.Data0,
            record.Data1,
            record.Data2,
            record.Data3,
            record.Created.ToDateTime().ToUniversalTime(),
            record.Updated.ToDateTime().ToUniversalTime());
        
        // Console.WriteLine($"Saved Order: {order.Id}");
    }
    
    // public async Task SaveReportAsync(TradeReport report)
    // {
    //     string executionTime = report.ExecutionTime.ToString("yyyy-MM-dd HH:mm:ss");
    //
    //     var reportSql =
    //         $"INSERT INTO \"TradeReports\" (\"Id\", \"OrderId\", \"Symbol\", \"ExecutedPrice\", \"ExecutedQuantity\", \"ExecutionTime\", \"ClientName\", \"Status\") VALUES ('{report.Id}', '{report.OrderId}', '{report.Symbol}', {report.ExecutedPrice}, {report.ExecutedQuantity}, '{executionTime}', '{report.ClientName}', {(int)report.Status})";
    //
    //     await context.Database.ExecuteSqlRawAsync(reportSql);
    //     // Console.WriteLine($"Saved Report: {report.Id}");
    // }
    //
    // public async Task SaveOrderAsync(CryptoOrder order)
    // {
    //     string orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");
    //
    //     var orderSql =
    //         $"INSERT INTO \"CryptoOrders\" (\"Id\", \"OrderId\", \"Symbol\", \"Price\", \"Quantity\", \"OrderDate\", \"Status\", \"ClientName\", \"Type\", \"StopLoss\", \"TakeProfit\") VALUES ('{order.Id}', '{order.OrderId}', '{order.Symbol}', {order.Price}, {order.Quantity}, '{orderDate}', {(int)order.Status}, '{order.ClientName}', {(int)order.Type}, {order.StopLoss}, {order.TakeProfit})";
    //
    //     await context.Database.ExecuteSqlRawAsync(orderSql);
    //     // Console.WriteLine($"Saved Order: {order.Id}");
    // }

}
