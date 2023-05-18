using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExecutionBenchmark.Connections;
using ExecutionBenchmark.Models;
using ExecutionBenchmark.Services;

namespace ExecutionBenchmark.Tests
{
    public static class BasicTest
    {
        public static async Task RabbitMq()
        {
            var stopwatch = new Stopwatch();
            
            var order = GetSampleOrder();
            var service = new RabbitMqService();
            
            stopwatch.Start();
            service.QueueOrder(order);
            stopwatch.Stop();
            Console.WriteLine($"RabbitMq: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task PostgresRawSql()
        {
            var stopwatch = new Stopwatch();

            var order = GetSampleOrder();
            var service = new RawSqlDbService(Constants.PostgresConnectionString);
            
            stopwatch.Start();
            await service.SaveOrderCommandAsync(order);
            stopwatch.Stop();
            Console.WriteLine($"PostgresRawSql: {stopwatch.ElapsedMilliseconds} ms");

        }
        
        public static async Task DannyRabbitMq()
        {
            var stopwatch = new Stopwatch();
            
            var record = Generator.GetSampleRecords(1).First();
            var factory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = "localhost",
            };
            var connection = new RabbitMqConnection(factory);
            await connection.EnsureAsync();
            await connection.ClearAsync();

            stopwatch.Start();
            await connection.SaveAsync(record);
            stopwatch.Stop();
            Console.WriteLine($"DannyRabbitMq: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task DannyPostgre()
        {
            var stopwatch = new Stopwatch();
            
            var connection = new PostgreConnection(new PostgreContext());
            await connection.EnsureAsync();
            await connection.ClearAsync();
            
            var record = Generator.GetSampleRecords(1).First();

            stopwatch.Start();
            await connection.SaveAsync(record);
            stopwatch.Stop();
            Console.WriteLine($"DannyPostgre: {stopwatch.ElapsedMilliseconds} ms");

        }
        
        public static async Task RhPostgre()
        {
            var stopwatch = new Stopwatch();

            var service = new RawSqlDbService(Constants.PostgresConnectionString);

            var record = Generator.GetSampleRecords(1).First();
            stopwatch.Start();
            await service.SaveRecordAsync(record);
            stopwatch.Stop();
            Console.WriteLine($"RhPostgre: {stopwatch.ElapsedMilliseconds} ms");

        }


        public static async Task LZ4()
        {
            var order = GetSampleOrder();
            await MeasureExecutionTime("LZ4", async () =>
            {
                await Task.Run(() => FileService.Lz4SaveAsync(order));
            });
        }
        
        public static async Task Kafka()
        {
            var order = GetSampleOrder();
            var kafkaService = new KafkaService();
            await MeasureExecutionTime("Kafka", async () =>
            {
                await Task.Run(() => kafkaService.QueueOrder(order));
            });
        }

        private static async Task MeasureExecutionTime(string testName, Func<Task> action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await action.Invoke();

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"{testName}: {elapsedMilliseconds} ms");
        }

        private static CryptoOrder GetSampleOrder()
        {
            var order = Generator.GetSampleCryptoOrders(1).First();
            order.Id = Guid.NewGuid();

            return order;
        }
    }
}
