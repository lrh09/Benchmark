using Google.Protobuf;
using RabbitMQ.Client;

namespace ExecutionBenchmark.Connections
{
    //rabbitmq connection
    public class RabbitMqConnection : IBenchmarkConnection
    {
        private const string QueueName = "my_queue";
        private readonly ConnectionFactory _factory;
        IConnection _connection;
        // Create a channel
        IModel _channel;
        public RabbitMqConnection(ConnectionFactory factory)
        {
            _factory = factory;
            // Create a connection
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }


        public async Task EnsureAsync()
        {
            // Create a connection
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declare a queue to publish to
                var queueName = QueueName;
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
        }

        public async Task ClearAsync()
        {
            // Create a connection
            using (var connection = _factory.CreateConnection())
            {
                // Create a channel
                using (var channel = connection.CreateModel())
                {
                    // Declare a queue to publish to
                    var queueName = QueueName;
                    channel.QueueDelete(queueName);
                }
            }
        }

        //publish item to rabbit mq
        public async Task SaveAsync(Record record)
        {
            // Serialize the record object 
            byte[] messageBytes = record.ToByteArray();
            // Publish the message
            _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: messageBytes);
        }
    }
}
