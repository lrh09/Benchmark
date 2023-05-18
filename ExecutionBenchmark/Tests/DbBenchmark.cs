using BenchmarkDotNet.Attributes;
using ExecutionBenchmark.Connections;
using ExecutionBenchmark.Models;
using ExecutionBenchmark.Services;

namespace ExecutionBenchmark.Tests;

[HtmlExporter]
public class DbBenchmark
{
    private readonly List<CryptoOrder> _orders = Generator.GetSampleCryptoOrders(1);
    private readonly List<TradeReport> _reports = Generator.GetSampleTradeReports(100);
    private readonly List<Record> _records = Generator.GetSampleRecords(100);
    
    private CryptoOrder Order = Generator.GetSampleCryptoOrders(1).First();
    private TradeReport Report = Generator.GetSampleTradeReports(1).First();
    
    private RabbitMqService _rabbitMqService;
    private RawSqlDbService _rawSqlDbService;
    
    private PostgreConnection _postgreConnection;
    private RabbitMqConnection _rabbitMqConnection;

    
    // private ScyllaDbService _scyllaDbService;
    //
    // private string _scyllaHost = "<Your ScyllaDB Host>";

    [Params(1, 50)] public int NumberOfOrders;

    [GlobalSetup]
    public void Setup()
    {
        _rawSqlDbService = new RawSqlDbService(Constants.PostgresConnectionString);
        _rabbitMqService = new RabbitMqService();

        SetupPostgre().Wait();
        SetupRabbitMq().Wait();


        // _dapperDbService = new DapperDbService(Constants.PostgresConnectionString);
        // _scyllaDbService = new ScyllaDbService(_scyllaHost);
        // _kafkaService = new KafkaService();
    }

    [Benchmark]
    public async Task RawSqlTest()
    {
        foreach (var record in _records)
        {
            await _rawSqlDbService.SaveRecordAsync(record);
        }
    }

    [Benchmark]
    public async Task DannyPostgreTest()
    {
        foreach (var record in _records)
        {
            await _postgreConnection.SaveAsync(record);
        }
    }

    [Benchmark]
    public void RabbitMqTest()
    {
        foreach (var record in _records)
        {
            _rabbitMqService.QueueRecord(record);
        }
    }
    
    [Benchmark]
    public async Task DannyRabbitMqTest()
    {
        foreach (var record in _records)
        {
            await _rabbitMqConnection.SaveAsync(record);
        }
    }
    
    
    
    // [Benchmark]
    // public void LZ4Test()
    // {
    //     foreach (var order in _orders)
    //     {
    //         order.Id = Guid.NewGuid();
    //         FileService.Lz4SaveAsync(order);
    //     }
    // }
    //
    // [Benchmark]
    // public void PlainFileTest()
    // {
    //     foreach (var order in _orders)
    //     {
    //         order.Id = Guid.NewGuid();
    //         FileService.PlainFileSaveAsync(order);
    //     }
    // }
    
    // [Benchmark]
    // public void KafkaTest()
    // {
    //     foreach (var order in _orders)
    //     {
    //         order.Id = Guid.NewGuid();
    //         FileService.PlainFileSaveAsync(order);
    //     }
    // }

    // [Benchmark]
    // public async Task ScyllaTest()
    // {
    //     foreach (var order in Generator.GetSampleCryptoOrders(100))
    //     {
    //         foreach (var report in Generator.GetSampleTradeReports(100).Where(r => r.OrderId == order.OrderId))
    //         {
    //             await _scyllaDbService.SaveOrderAndReportAsync(order, report);
    //         }
    //     }
    // }
    
    // [Benchmark]
    // public async Task DapperTest()
    // {
    //     foreach (var order in _orders)
    //     {
    //         order.Id = Guid.NewGuid();
    //         await _dapperDbService.SaveOrderAsync(order);
    //     }
    // }
    
    private async Task SetupPostgre()
    {
        _postgreConnection = new PostgreConnection(new PostgreContext());
        await _postgreConnection.EnsureAsync();
        await _postgreConnection.ClearAsync();
    }
    
    private async Task SetupRabbitMq()
    {
        var factory = new RabbitMQ.Client.ConnectionFactory()
        {
            HostName = "localhost"
        };
        _rabbitMqConnection = new RabbitMqConnection(factory);
        await _rabbitMqConnection.EnsureAsync();
        await _rabbitMqConnection.ClearAsync();
    }
}