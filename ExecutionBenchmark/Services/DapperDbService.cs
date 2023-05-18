using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ExecutionBenchmark.Models;
using Npgsql;

namespace ExecutionBenchmark.Services;

public class DapperDbService
{
    private readonly IDbConnection _connection;

    public DapperDbService(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString); // or SqlConnection for MSSQL
    }

    public async Task SaveOrderAndReportAsync(CryptoOrder order, TradeReport report)
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        using var transaction = _connection.BeginTransaction();
        try
        {
            await SaveOrderAsync(order);
            await SaveReportAsync(report);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task SaveReportAsync(TradeReport report)
    {
        var reportSql =
            "INSERT INTO \"TradeReports\" (\"Id\", \"OrderId\", \"Symbol\", \"ExecutedPrice\", \"ExecutedQuantity\", \"ExecutionTime\", \"ClientName\", \"Status\") VALUES (@Id, @OrderId, @Symbol, @ExecutedPrice, @ExecutedQuantity, @ExecutionTime, @ClientName, @Status)";
        await _connection.ExecuteAsync(reportSql, report);
        
        // Console.WriteLine($"Saved Report: {report.Id}");
    }

    public async Task SaveOrderAsync(CryptoOrder order)
    {
        var orderSql =
            "INSERT INTO \"CryptoOrders\" (\"Id\", \"OrderId\", \"Symbol\", \"Price\", \"Quantity\", \"OrderDate\", \"Status\", \"ClientName\", \"Type\", \"StopLoss\", \"TakeProfit\") VALUES (@Id, @OrderId, @Symbol, @Price, @Quantity, @OrderDate, @Status, @ClientName, @Type, @StopLoss, @TakeProfit)";
        await _connection.ExecuteAsync(orderSql, order);
        
        // Console.WriteLine($"Saved Order: {order.Id}");
    }
}