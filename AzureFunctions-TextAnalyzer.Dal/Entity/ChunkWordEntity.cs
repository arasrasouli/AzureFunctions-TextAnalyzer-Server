using Azure;
using Azure.Data.Tables;
using System.Text.Json;

namespace AzureFunctions_TextAnalyzer.DAL.Entities
{
    public class ChunkWordEntity : ITableEntity
    {
        public ChunkWordEntity() { }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string WordsCount { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }


        public ChunkWordEntity(string partitionKey, string rowKey, string wordsCount)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            WordsCount = wordsCount;
        }
    }
}
