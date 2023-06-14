using BenchmarkDotNet.Attributes;
using ExecutionBenchmark.Connections;
using ExecutionBenchmark.Models;
using ExecutionBenchmark.Services;
using Environment = ExecutionBenchmark.Services.Environment;

namespace ExecutionBenchmark.Tests;

// [HtmlExporter]
public class DbBenchmark
{
    private readonly Environment _environment = Environment.Local;
    
    private readonly List<TradeReport> _reports = Generator.GetSampleTradeReports(100);
    private TradeReport Report = Generator.GetSampleTradeReports(1).First();
    
    private readonly List<CryptoOrder> _orders = Generator.GetSampleCryptoOrders(1);
    private CryptoOrder Order = Generator.GetSampleCryptoOrders(1).First();
    
    private RabbitMqService _rabbitMqService;
    private RawSqlDbService _rawSqlDbService;
    

    [Params(1, 50)] public int NumberOfOrders;

    [GlobalSetup]
    public void Setup()
    {
        _rawSqlDbService = new RawSqlDbService(_environment);
        _rabbitMqService = new RabbitMqService(_environment);
    }

    [Benchmark]
    public async Task RawSqlTest()
    {
        foreach (var order in _orders)
        {
            await _rawSqlDbService.SaveOrderCommandAsync(order);
        }
    }

    [Benchmark]
    public void RabbitMqTest()
    {
        foreach (var order in _orders)
        {
            _rabbitMqService.QueueOrder(order);
        }
    }
}