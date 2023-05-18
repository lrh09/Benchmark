using System;
using System.Text;
using Confluent.Kafka;
using ExecutionBenchmark.Models;
using Newtonsoft.Json;

namespace ExecutionBenchmark.Services;

public class KafkaService
{


    private readonly IProducer<Null, byte[]> _producer;
    private IConsumer<Ignore, string> _consumer;

    public KafkaService()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "KafkaSenderApp"
        };
        _producer = new ProducerBuilder<Null, byte[]>(config).Build();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "host.docker.internal:9092",
            GroupId = Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
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

        _producer.ProduceAsync(Constants.KafkaReportTopicName, new Message<Null, byte[]> { Key = null, Value = reportBody });
    }

    public void QueueOrder(CryptoOrder order)
    {
        var orderMessage = JsonConvert.SerializeObject(order);
        var orderBody = Encoding.UTF8.GetBytes(orderMessage);

        _producer.ProduceAsync(Constants.KafkaOrderTopicName, new Message<Null, byte[]> { Key = null, Value = orderBody });
    }
}