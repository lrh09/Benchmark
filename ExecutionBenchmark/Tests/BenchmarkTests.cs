using BenchmarkDotNet.Attributes;
using ExecutionBenchmark.Models;
using ExecutionBenchmark.Services;
using Environment = ExecutionBenchmark.Services.Environment;

namespace ExecutionBenchmark.Tests;

public class BenchmarkTests
{
    private readonly Environment _environment = Environment.Local;
    
    private readonly RawSqlDbService _rawSqlDbService;
    private readonly RabbitMqService _rabbitMqService;
    private CryptoOrder _order = Generator.GetSampleCryptoOrders(1).First();

    public BenchmarkTests()
    {
        // Initialize services
        _rawSqlDbService = new RawSqlDbService(_environment);
        _rabbitMqService = new RabbitMqService(_environment);
    }

    [GlobalSetup]
    public void Setup()
    {
        // Perform setup logic if needed
    }

    // [GlobalCleanup]
    // public void Cleanup()
    // {
    //     // Clean up logic after all benchmarks have run
    //     Task.Run(() => _rawSqlDbService.CleanUpAsync()).Wait();
    //     _rabbitMqService.CleanUpAsync();
    // }

    [Benchmark]
    public void InsertWithRawSql()
    {
        Task.Run(() => _rawSqlDbService.SaveOrderCommandAsync(_order)).Wait();
    }

    [Benchmark]
    public void QueueWithRabbitMq()
    {
        _rabbitMqService.QueueOrder(_order);
    }
}