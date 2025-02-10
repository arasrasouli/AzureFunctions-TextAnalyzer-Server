using Azure.Data.Tables;
using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public class ChunkWordRepository : TableStorageRepository<ChunkWordEntity>, IChunkWordRepository
    {
        public ChunkWordRepository(TableClient tableClient)
            : base(tableClient)
        {
        }
        public async Task<List<string>> GetChunksAsync(string partitionKey)
        {
            var queryResults = _tableClient.QueryAsync<ChunkWordEntity>(e => e.PartitionKey == partitionKey);

            List<string> chunkRowsDictionary = new List<string>();
            await foreach (ChunkWordEntity row in queryResults)
            {
                chunkRowsDictionary.Add(row.WordsCount);
            }

            return chunkRowsDictionary;
        }
    }
}
