using Bogus;
using ExecutionBenchmark.Models;
using Google.Protobuf.WellKnownTypes;

namespace ExecutionBenchmark.Tests;

public static class Generator
{
    private static readonly Random _rng = new Random();
    private static readonly List<string> _symbols = new() { "BTCUSD", "ETHUSD", "LTCUSD", "XRPUSD", "BNBUSD" };
    private static readonly List<string> _clientNames = new() { "Alice", "Bob", "Charlie", "David", "Eve" };

    public static List<CryptoOrder> GetSampleCryptoOrders(int n)
    {
        var orders = new List<CryptoOrder>();
        for (var i = 0; i < n; i++)
        {
            var orderId = Guid.NewGuid().ToString();
            var symbol = _symbols[_rng.Next(_symbols.Count)];
            var price = (decimal)_rng.NextDouble() * 50000;
            var quantity = (decimal)_rng.NextDouble() * 10;
            var clientName = _clientNames[_rng.Next(_clientNames.Count)];
            var orderType = (CryptoOrder.OrderType)_rng.Next(2);
            var status = (CryptoOrder.OrderStatus)_rng.Next(3);
            var stopLoss = (decimal)_rng.NextDouble() * 50000;
            var takeProfit = (decimal)_rng.NextDouble() * 50000;
            var orderDate = DateTime.UtcNow;

            orders.Add(new CryptoOrder
            {
                OrderId = orderId,
                Symbol = symbol,
                Price = price,
                Quantity = quantity,
                OrderDate = orderDate,
                Status = status,
                ClientName = clientName,
                Type = orderType,
                StopLoss = stopLoss,
                TakeProfit = takeProfit
            });
        }

        return orders;
    }

    public static List<TradeReport> GetSampleTradeReports(int n)
    {
        var _reports = new List<TradeReport>();
        for (var i = 0; i < n; i++)
        {
            var orderId = Guid.NewGuid().ToString();
            var symbol = _symbols[_rng.Next(_symbols.Count)];
            var price = (decimal)_rng.NextDouble() * 50000;
            var quantity = (decimal)_rng.NextDouble() * 10;
            var clientName = _clientNames[_rng.Next(_clientNames.Count)];
            var tradeStatus = (TradeReport.TradeStatus)_rng.Next(3);

            _reports.Add(new TradeReport
            {
                OrderId = orderId,
                Symbol = symbol,
                ExecutedPrice = price,
                ExecutedQuantity = quantity,
                ExecutionTime = DateTime.UtcNow,
                ClientName = clientName,
                Status = tradeStatus
            });
        }

        return _reports;
    }

    public static List<Record> GetSampleRecords(int n) =>
        new Faker<Record>()
            .StrictMode(true)
            .RuleFor(r => r.Id, f => f.IndexFaker)
            .RuleFor(r => r.Uuid, f => f.Random.Guid().ToString())
            .RuleFor(r => r.Name, f => f.Name.FullName())
            .RuleFor(r => r.Data0, f => f.Lorem.Sentence())
            .RuleFor(r => r.Data1, f => f.Lorem.Sentence(2))
            .RuleFor(r => r.Data2, f => f.Lorem.Sentence(5))
            .RuleFor(r => r.Data3, f => f.Lorem.Sentence(10))
            .RuleFor(r => r.Created, f => f.Date.Past().ToUniversalTime().ToTimestamp())
            .RuleFor(r => r.Updated, f => f.Date.Recent().ToUniversalTime().ToTimestamp())
            .Generate(n);
}