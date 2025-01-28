namespace AzureFunctions_TextAnalyzer.Functions.Dto
{
    public class QueueMessageDto
    {
        public string FileName { get; set; }
        public long StartPoint { get; set; }
        public long EndPoint { get; set; }
        public int ChunkOrder { get; set; }
        public int LastChunkOrder { get; set; }
    }

    public class ChunkProcessingQueueMessageDto
    {
        public string FileName { get; set; }
        public Dictionary<string, int> WordCounts { get; set; }
        public int ChunkOrder { get; set; }
        public int LastChunkOrder { get; set; }
    }
}
