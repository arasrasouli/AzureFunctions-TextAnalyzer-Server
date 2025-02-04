using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public interface IFileChunkRepository : ITableStorageRepository<FileChunkEntity>
    {
        Task<FileChunkEntity?> UpdateRemainingChunksAsync(string partitionKey, string rowKey, int maxRetries);
    }
}