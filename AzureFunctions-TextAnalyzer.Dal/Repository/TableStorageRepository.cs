using Azure.Data.Tables;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public class TableStorageRepository<T> : ITableStorageRepository<T> where T : class, ITableEntity, new()
    {
        protected readonly TableClient _tableClient;

        public TableStorageRepository(string connectionString, string tableName)
        {
            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task AddEntityAsync(T entity)
        {
            await _tableClient.AddEntityAsync(entity);
        }

        public async Task DeleteEntityAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            return await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }
    }
}
