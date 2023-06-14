using System;
using System.Text;
using ExecutionBenchmark.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ExecutionBenchmark.Services;

public class RabbitMqService
{
    private readonly IModel _channel;
    private const string DevHostName = "pe-dev";
    private static readonly ConnectionFactory LocalFactory = new()
    {
        Uri = new Uri("amqp://guest:guest@localhost:5672"),
        ClientProvidedName = "RabbitSenderApp"
    };
    private static readonly ConnectionFactory DevFactory = new()
    {
        Uri = new Uri($"amqp://ruennhuah:80mbTfv55izv1Plbq7vz@{DevHostName}:5672"),
    };

    public RabbitMqService(Environment environment)
    {
        var connection = environment switch
        {
            Environment.Local => LocalFactory.CreateConnection(),
            Environment.DevServer => DevFactory.CreateConnection(),
            _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, null)
        };

        _channel = connection.CreateModel();
        
        DeclareExchangeAndQueues();
    }

    private void DeclareExchangeAndQueues()
    {
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
    
    public void CleanUp()
    {
        try
        {
            // Delete the exchanges
            _channel.ExchangeDelete(Constants.RabbitExchangeName);

            // Purge and delete the queues
            _channel.QueuePurge(Constants.OrderQueueName);
            _channel.QueueDelete(Constants.OrderQueueName);

            _channel.QueuePurge(Constants.ReportQueueName);
            _channel.QueueDelete(Constants.ReportQueueName);
            
            _channel.QueuePurge(Constants.RecordQueueName);
            _channel.QueueDelete(Constants.RecordQueueName);

            Console.WriteLine("Successfully cleaned up RabbitMQ exchanges, queues, and messages.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while cleaning up RabbitMQ: {ex.Message}");
        }
    }

    
}

public enum Environment
{
    Local = 1,
    DevServer = 2
}