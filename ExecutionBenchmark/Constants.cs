namespace ExecutionBenchmark;

public static class Constants
{
    public const string PostgresConnectionString = "Host=localhost;Port=5430;Database=postgres;Username=postgres;Password=Random123!";
    public const string PostgresConnectionStringDev = "Host=pe-dev;Port=5432;Database=test;Username=developer;Password=iPQ339YIGtHOzYchHblq";
    public const string MsSqlConnectionString = "Server=localhost,1436;Database=master;User Id=sa;Password=Random123!;TrustServerCertificate=true";

    public const string RabbitExchangeName = "MyExchange";
    public const string OrderQueueName = "CryptoOrders";
    public const string OrderRoutingKey = "orders";
    public const string ReportQueueName = "TradeReports";
    public const string ReportRoutingKey = "reports";
    public const string RecordQueueName = "RH-Records";
    public const string RecordRoutingKey = "rh-records";

    public const string KafkaOrderTopicName = "OrderTopic";
    public const string KafkaReportTopicName = "ReportTopic";

}