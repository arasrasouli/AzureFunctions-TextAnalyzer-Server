namespace AzureFunctions_TextAnalyzer.Service.Model
{
    public class ChunkDataModel
    {
        public long StartPoint { get; set; }
        public long EndPoint { get; set; }
        public int ChunkIndex { get; set; }
        public int ChunksCount { get; set; }
    }

    public class ChunkProcessingModel
    {
        public Dictionary<string, int> WordCounts { get; set; }
        public int ChunkIndex { get; set; }
    }
}
