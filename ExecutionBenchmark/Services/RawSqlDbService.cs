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
    private readonly NpgsqlConnection _connection;
    private readonly PostgresDbContext _context = new();

    public RawSqlDbService(Environment environment)
    {
        _connection = GetOpenConnection(environment);
        _connection = GetOpenConnection(environment);
    }

    public NpgsqlConnection GetOpenConnection(Environment environment)
    {
        var connectionString = GetConnectionString(environment);
        var connection = new NpgsqlConnection(connectionString);

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return connection;
    }

    private string GetConnectionString(Environment environment)
    {
        // Retrieve the connection string based on the environment
        return environment switch
        {
            Environment.Local => Constants.PostgresConnectionString,
            Environment.DevServer => Constants.PostgresConnectionStringDev,
            _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, null)
        };
    }

    public async Task SaveReportCommandAsync(TradeReport report)
    {
        string executionTime = report.ExecutionTime.ToString("yyyy-MM-dd HH:mm:ss");

        var reportSql =
            string.Format(
                "INSERT INTO \"TradeReports\" (\"Id\", \"OrderId\", \"Symbol\", \"ExecutedPrice\", \"ExecutedQuantity\", \"ExecutionTime\", \"ClientName\", \"Status\") VALUES ('{0}', '{1}', '{2}', {3}, {4}, '{5}', '{6}', {7})",
                report.Id, report.OrderId, report.Symbol, report.ExecutedPrice, report.ExecutedQuantity, executionTime,
                report.ClientName, (int)report.Status);

        using var reportCommand = new NpgsqlCommand(reportSql, (NpgsqlConnection)_connection); // or SqlCommand for MSSQL
        await reportCommand.ExecuteNonQueryAsync();

        // Console.WriteLine($"Saved Report: {report.Id}");
    }
    
    public async Task SaveOrderCommandAsync(CryptoOrder order, NpgsqlConnection connection)
    {
        string orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");

        var orderSql =
            string.Format(
                "INSERT INTO \"CryptoOrders\" (\"Id\", \"OrderId\", \"Symbol\", \"Price\", \"Quantity\", \"OrderDate\", \"Status\", \"ClientName\", \"Type\", \"StopLoss\", \"TakeProfit\") VALUES ('{0}', '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, {10})",
                order.Id, order.OrderId, order.Symbol, order.Price, order.Quantity, orderDate, (int)order.Status,
                order.ClientName, (int)order.Type, order.StopLoss, order.TakeProfit);

        using var orderCommand = new NpgsqlCommand(orderSql, connection); // or SqlCommand for MSSQL
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

    public async Task CleanUpAsync()
    {
        await TruncateTableAsync("CryptoOrders");
        await TruncateTableAsync("TradeReports");
    }

    private async Task TruncateTableAsync(string tableName)
    {
        try
        {
            var sqlCommand = $"TRUNCATE TABLE \"{tableName}\";";
            await using var truncateCommand = new NpgsqlCommand(sqlCommand, _connection);
            await truncateCommand.ExecuteNonQueryAsync();

            Console.WriteLine($"Successfully truncated {tableName} table.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

}
