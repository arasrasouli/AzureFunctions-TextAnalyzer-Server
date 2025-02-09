using Azure.Data.Tables;
using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public class FileChunkRepository : TableStorageRepository<FileChunkEntity>, IFileChunkRepository
    {
        public FileChunkRepository(TableClient tableClient)
            : base(tableClient)
        {
        }

        public async Task<FileChunkEntity?> UpdateRemainingChunksAsync(string partitionKey, string rowKey, int maxRetries)
        {
            bool success = false;
            int retryCount = 0;

            while (!success && retryCount < maxRetries)
            {
                try
                {
                    FileChunkEntity entity = await _tableClient.GetEntityAsync<FileChunkEntity>(partitionKey, rowKey, null);

                    if (entity == null)
                    {
                        Console.WriteLine($"Entity not found: {partitionKey} - {rowKey}");
                        return null;
                    }

                    entity.RemainingChunks -= 1;
                    await _tableClient.UpdateEntityAsync(entity, entity.ETag);

                    success = true;
                    Console.WriteLine($"Countdown decremented successfully: {partitionKey} - {rowKey}");
                    return entity;
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 412)
                {
                    Console.WriteLine($"Optimistic concurrency conflict. Retrying... {partitionKey} - {rowKey}");
                    retryCount++;
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * retryCount));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            Console.WriteLine($"Failed to update after {maxRetries} retries: {partitionKey} - {rowKey}");
            return null;
        }
    }
}
