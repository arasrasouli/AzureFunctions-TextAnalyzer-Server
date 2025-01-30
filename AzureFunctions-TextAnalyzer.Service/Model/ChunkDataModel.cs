namespace AzureFunctions_TextAnalyzer.Service.Model
{
    public class ChunkDataModel
    {
        public string Name { get; set; }
        public long StartPoint { get; set; }
        public long EndPoint { get; set; }
        public int ChunkIndex { get; set; }
        public int ChunksCount { get; set; }
    }
}
