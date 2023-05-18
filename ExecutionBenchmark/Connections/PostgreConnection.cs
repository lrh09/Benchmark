using Microsoft.EntityFrameworkCore;

namespace ExecutionBenchmark.Connections
{


    internal class PostgreConnection : IBenchmarkConnection
    {
        private readonly DbContext _dbContext;

        public PostgreConnection(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task EnsureAsync()
        {
            //create record table
            _dbContext.Database.EnsureCreated();
            await Task.CompletedTask;
        }

        public async Task ClearAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"Records\" RESTART IDENTITY RESTRICT;");
        }

        public async Task SaveAsync(Record record)
        {

            //saving record into postgre database using rawsql
            await _dbContext.Database.ExecuteSqlRawAsync(
                "INSERT INTO public.\"Records\" (\"Uuid\", \"Name\", \"Data0\", \"Data1\", \"Data2\", \"Data3\", \"Created\", \"Updated\") " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});",
                record.Uuid,
                record.Name,
                record.Data0,
                record.Data1,
                record.Data2,
                record.Data3,
                record.Created.ToDateTime().ToUniversalTime(),
                record.Updated.ToDateTime().ToUniversalTime());

        }

    }
}
