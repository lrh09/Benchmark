using System;
using System.Text;
using ExecutionBenchmark.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ExecutionBenchmark.Services;

public class RabbitMqService
{
    private readonly IModel _channel;

    public RabbitMqService()
    {
        ConnectionFactory factory = new();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5678");
        factory.ClientProvidedName = "RabbitSenderApp";
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        
        // Create the exchange
        _channel.ExchangeDeclare(Constants.RabbitExchangeName, ExchangeType.Direct, durable: true);

        // Declare queues and bind them to the exchange
        _channel.QueueDeclare(Constants.OrderQueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(Constants.OrderQueueName, Constants.RabbitExchangeName, Constants.OrderRoutingKey);

        _channel.QueueDeclare(Constants.ReportQueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(Constants.ReportQueueName, Constants.RabbitExchangeName, Constants.ReportRoutingKey);
    
        _channel.QueueDeclare(Constants.RecordQueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(Constants.RecordQueueName, Constants.RabbitExchangeName, Constants.RecordRoutingKey);
    }

    public void QueueOrderAndReport(CryptoOrder order, TradeReport report)
    {
        QueueOrder(order);
        QueueReport(report);
    }

    public void QueueReport(TradeReport report)
    {
        var reportMessage = JsonConvert.SerializeObject(report);
        var reportBody = Encoding.UTF8.GetBytes(reportMessage);

        _channel.BasicPublish(Constants.RabbitExchangeName, Constants.ReportRoutingKey, null, reportBody);
    }

    public void QueueOrder(CryptoOrder order)
    {
        var orderMessage = JsonConvert.SerializeObject(order);
        var orderBody = Encoding.UTF8.GetBytes(orderMessage);

        _channel.BasicPublish(Constants.RabbitExchangeName, Constants.OrderRoutingKey, null, orderBody);
        // Console.WriteLine($"Queue: {Constants.RabbitExchangeName}| {Constants.ReportRoutingKey} |{orderMessage}");
    }

    public void QueueRecord(Record record)
    {
        var message = JsonConvert.SerializeObject(record);
        var body = Encoding.UTF8.GetBytes(message);
        
        _channel.BasicPublish(Constants.RabbitExchangeName, Constants.RecordRoutingKey, null, body);
    }
}