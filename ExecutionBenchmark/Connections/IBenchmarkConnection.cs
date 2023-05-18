namespace ExecutionBenchmark.Connections
{
    public interface IBenchmarkConnection
    {
        Task ClearAsync();
        Task EnsureAsync();
        Task SaveAsync(Record record);
    }
}