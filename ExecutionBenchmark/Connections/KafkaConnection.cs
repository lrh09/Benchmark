using Confluent.Kafka;
using Google.Protobuf;

namespace ExecutionBenchmark.Connections
{
    internal class KafkaConnection : IBenchmarkConnection
    {
        static string Topic = "quickstart";
        private IProducer<Null, byte[]> _producer;
        private IConsumer<Ignore, string> _consumer;
        public KafkaConnection()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false
            };
            _producer = new ProducerBuilder<Null, byte[]>(config).Build();
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            //Task.Run(() =>
            //{
            //    var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            //    consumer.Subscribe(Topic);

            //    try
            //    {
            //        var i = 0;
            //        while (true)
            //        {
            //            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
            //            i++;
            //            if(i % 10 == 0)
            //            {
            //                Console.WriteLine(i);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error occurred: {ex.Message}");
            //    }
            //});
        }

        public async Task ClearAsync()
        {
            _producer.Flush();
            await Task.CompletedTask;
            //await ClearAllMessagesAsync();
        }

        public async Task ClearAllMessagesAsync()
        {
            _consumer.Subscribe(Topic);

            try
            {
                var i = 0;
                while (true)
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult == null)
                    {
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        public Task EnsureAsync()
        {
            return Task.CompletedTask;
        }

        public async Task SaveAsync(Record record)
        {

            var message = new Message<Null, byte[]>
            {
                Value = record.ToByteArray()
            };
            try
            {
                var report = await _producer.ProduceAsync(Topic, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
