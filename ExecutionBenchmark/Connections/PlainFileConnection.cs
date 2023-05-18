using Google.Protobuf;

namespace ExecutionBenchmark.Connections
{
    internal class PlainFileConnection : IBenchmarkConnection
    {
        static string _filePath = "records.file";

        public Task ClearAsync()
        {
            File.Delete(_filePath);
            return Task.CompletedTask;
        }

        public Task EnsureAsync()
        {
            return Task.CompletedTask;
        }

        public async Task SaveAsync(Record record)
        {
            byte[] data = record.ToByteArray();

            using FileStream fileStream = new FileStream(_filePath, FileMode.OpenOrCreate);
            await fileStream.WriteAsync(data, 0, data.Length);
        }
    }
}
