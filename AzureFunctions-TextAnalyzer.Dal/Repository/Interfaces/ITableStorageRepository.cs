using Azure.Data.Tables;
using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public interface ITableStorageRepository<T> where T : class, ITableEntity, new()
    {
        Task AddEntityAsync(T entity);
        Task DeleteEntityAsync(string partitionKey, string rowKey);
        Task<T> GetEntityAsync(string partitionKey, string rowKey);
    }
}
