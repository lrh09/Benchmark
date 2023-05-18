using BenchmarkDotNet.Attributes;
using ExecutionBenchmark.Connections;

namespace ExecutionBenchmark.Tests
{

    [HtmlExporter]
    public class TheBenchmark
    {
        private List<Record> _records;

        // [Params(1,100)]
        // public int N;
        [GlobalSetup]
        public void Setup()
        {
            _records = Generator.GetSampleRecords(1);

            SetupPostgre().Wait();
            SetupRabbitMq().Wait();
            SetupKafa().Wait();
            SetupLz4().Wait();
            SetupPlain().Wait();
        }

        private PostgreConnection _postgreConnection;
        private RabbitMqConnection _rabbitMqConnection;
        private KafkaConnection _kafkaConnection;
        private LZ4FileConnection _lz4Connection;
        private PlainFileConnection _plainFileConnection;

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

        private async Task SetupPostgre()
        {
            _postgreConnection = new PostgreConnection(new PostgreContext());
            await _postgreConnection.EnsureAsync();
            await _postgreConnection.ClearAsync();
        }

        private async Task SetupKafa()
        {
            _kafkaConnection = new KafkaConnection();
        }

        private async Task SetupLz4()
        {
            _lz4Connection = new LZ4FileConnection();
        }

        private async Task SetupPlain()
        {
            _plainFileConnection = new PlainFileConnection();
        }

        [Benchmark]
        public async Task InsertPgsql()
        {
            foreach(var r in _records) 
            { 
                await _postgreConnection.SaveAsync(r);
            }
        }

        [Benchmark]
        public async Task InsertRabbitMQ()
        {
            foreach (var r in _records)
            {
                await _rabbitMqConnection.SaveAsync(r);
            }
        }

        // [Benchmark]
        public async Task InsertKafa()
        {
            foreach (var r in _records)
            {
                await _kafkaConnection.SaveAsync(r);
            }
        }

        // [Benchmark]
        public async Task InsertLZ4File()
        {
            foreach (var r in _records)
            {
                await _lz4Connection.SaveAsync(r);
            }
        }

        // [Benchmark]
        public async Task InsertPlainFile()
        {
            foreach (var r in _records)
            {
                await _plainFileConnection.SaveAsync(r);
            }
        }

        // [GlobalCleanup]
        public async Task GlobalCleanUp()
        {
            await _postgreConnection.ClearAsync();
            await _rabbitMqConnection.ClearAsync();
        }
    }
}
