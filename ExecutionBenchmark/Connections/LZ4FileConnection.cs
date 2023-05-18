using Google.Protobuf;
using K4os.Compression.LZ4.Streams;

namespace ExecutionBenchmark.Connections
{
    internal class LZ4FileConnection : IBenchmarkConnection
    {
        static string _filePath = "records.lz4";


        public LZ4FileConnection()
        {

        }
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

            using (FileStream fileStream = new FileStream(_filePath, FileMode.OpenOrCreate))
            using (var lz4Stream = LZ4Stream.Encode(fileStream, K4os.Compression.LZ4.LZ4Level.L00_FAST))
            {
                await lz4Stream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
