namespace AzureFunctions_TextAnalyzer.Common
{
    public interface IFileServices
    {
        Task<string> ReadBlobChunkAsync(string blobName, long startPoint, long endPoint);
    }
}