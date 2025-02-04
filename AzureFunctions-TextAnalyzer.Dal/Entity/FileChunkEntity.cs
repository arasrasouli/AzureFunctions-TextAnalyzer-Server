using Azure;
using Azure.Data.Tables;

namespace AzureFunctions_TextAnalyzer.DAL.Entities
{
    public class FileChunkEntity : ITableEntity
    {
        public FileChunkEntity() { }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int RemainingChunks { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }


        public FileChunkEntity(string partitionKey, string name, int remainingChunks)
        {
            PartitionKey = partitionKey;
            RowKey = name;
            RemainingChunks = RemainingChunks;
        }
    }
}
