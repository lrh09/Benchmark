using System.Diagnostics;
using ExecutionBenchmark.Models;
using ExecutionBenchmark.Services;
using Environment = ExecutionBenchmark.Services.Environment;

namespace ExecutionBenchmark.Tests
{
    public static class BasicTest
    {
        public static async Task RabbitMq(Environment environment)
        {
            var stopwatch = new Stopwatch();
            
            var order = GetSampleOrder();
            var service = new RabbitMqService(environment);
            
            stopwatch.Start();
            service.QueueOrder(order);
            stopwatch.Stop();
            Console.WriteLine($"RabbitMq: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task PostgresRawSql(Environment environment)
        {
            var stopwatch = new Stopwatch();

            var order = GetSampleOrder();
            var service = new RawSqlDbService(environment);
            
            stopwatch.Start();
            await service.SaveOrderCommandAsync(order, service.GetOpenConnection(environment));
            stopwatch.Stop();
            Console.WriteLine($"PostgresRawSql: {stopwatch.ElapsedMilliseconds} ms");

        }
        
        private static CryptoOrder GetSampleOrder()
        {
            var order = Generator.GetSampleCryptoOrders(1).First();
            order.Id = Guid.NewGuid();

            return order;
        }
    }
}
