using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExecutionBenchmark.Models;
using K4os.Compression.LZ4.Streams;
using Newtonsoft.Json;

namespace ExecutionBenchmark.Services;

public static class FileService
{
    public static async Task Lz4SaveAsync(CryptoOrder order)
    {
        const string _filePath = "record.lz4";
        
        var orderMessage = JsonConvert.SerializeObject(order);
        var orderBody = Encoding.UTF8.GetBytes(orderMessage);

        await using var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate);
        await using var lz4Stream = LZ4Stream.Encode(fileStream, K4os.Compression.LZ4.LZ4Level.L00_FAST);
        await lz4Stream.WriteAsync(orderBody);
    }
    
    public static async Task PlainFileSaveAsync(CryptoOrder order)
    {
        const string _filePath = "record.file";
        
        var orderMessage = JsonConvert.SerializeObject(order);
        var orderBody = Encoding.UTF8.GetBytes(orderMessage);

        await using var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate);
        await fileStream.WriteAsync(orderBody);
    }
}