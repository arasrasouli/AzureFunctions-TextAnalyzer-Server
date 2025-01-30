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

    public class ChunkWordCountQueueMessageDto
    {
        public string FileName { get; set; }
        public Dictionary<string, int> WordCounts { get; set; }
        public int ChunkOrder { get; set; }
        public int ChunksCount { get; set; }
    }
}
