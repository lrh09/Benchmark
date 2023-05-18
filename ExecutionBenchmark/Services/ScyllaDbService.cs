using System.Threading.Tasks;
using Cassandra;
using ExecutionBenchmark.Models;

namespace ExecutionBenchmark.Services;

public class ScyllaDbService
{
    private readonly Cluster _cluster;
    private readonly ISession _session;

    public ScyllaDbService(string contactPoint)
    {
        _cluster = Cluster.Builder()
            .AddContactPoint(contactPoint)
            .Build();
        _session = _cluster.Connect("mykeyspace");  // Replace with your keyspace
    }

    public async Task SaveOrderAndReportAsync(CryptoOrder order, TradeReport report)
    {
        var orderStatement = new SimpleStatement(
            "INSERT INTO crypto_order (order_id, symbol, price, quantity, order_date, status, client_name, type, stop_loss, take_profit) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
            order.OrderId, order.Symbol, order.Price, order.Quantity, order.OrderDate, order.Status.ToString(), order.ClientName, order.Type.ToString(), order.StopLoss, order.TakeProfit);
        
        var reportStatement = new SimpleStatement(
            "INSERT INTO trade_report (id, order_id, symbol, executed_price, executed_quantity, execution_time, client_name, status) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
            report.Id, report.OrderId, report.Symbol, report.ExecutedPrice, report.ExecutedQuantity, report.ExecutionTime, report.ClientName, report.Status.ToString());

        await _session.ExecuteAsync(orderStatement);
        await _session.ExecuteAsync(reportStatement);
    }
}
