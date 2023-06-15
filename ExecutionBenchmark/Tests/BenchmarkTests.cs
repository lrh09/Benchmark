using BenchmarkDotNet.Attributes;
using ExecutionBenchmark.Models;
using ExecutionBenchmark.Services;
using Npgsql;
using Environment = ExecutionBenchmark.Services.Environment;

namespace ExecutionBenchmark.Tests;

public class BenchmarkTests
{
    private readonly Environment _environment = Environment.Local;
    
    private readonly RawSqlDbService _rawSqlDbService;
    private readonly RabbitMqService _rabbitMqService;
    private readonly List<CryptoOrder> _orders = Generator.GetSampleCryptoOrders(50);
    
    private readonly NpgsqlConnection _dbConnection;

    public BenchmarkTests()
    {
        _rawSqlDbService = new RawSqlDbService(_environment);
        _rabbitMqService = new RabbitMqService(_environment);
        
        _dbConnection = _rawSqlDbService.GetOpenConnection(_environment);
    }

    [Benchmark]
    public async Task InsertWithRawSql()
    {
        foreach (var order in _orders)
        {
            order.Id = Guid.NewGuid();
            await _rawSqlDbService.SaveOrderCommandAsync(order, _dbConnection);
        }
        
    }

    [Benchmark]
    public void QueueWithRabbitMq()
    {
        foreach (var order in _orders)
        {
            order.Id = Guid.NewGuid();
            _rabbitMqService.QueueOrder(order);
        }
    }
}
