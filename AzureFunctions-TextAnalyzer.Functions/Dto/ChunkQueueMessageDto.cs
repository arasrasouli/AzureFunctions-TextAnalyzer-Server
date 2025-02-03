namespace AzureFunctions_TextAnalyzer.Functions.Dto
{
    public class ChunkQueueMessageDto
    {
        public string FileName { get; set; }
        public long StartPoint { get; set; }
        public long EndPoint { get; set; }
        public int ChunkIndex { get; set; }
        public int ChunksCount { get; set; }
    }

    public class FileChunkDto
    {
        public string PartitionKey { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
    }
}
