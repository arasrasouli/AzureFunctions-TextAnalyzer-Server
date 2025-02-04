using System.Text;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace AzureFunctions_TextAnalyzer.Common
{
    public class FileServices : IFileServices
    {
        private readonly ILogger<FileServices> _logger;
        private readonly string _connectionString;
        private readonly string _containerName;

        public FileServices(ILogger<FileServices> logger, string connectionString, string containerName)
        {
            _logger = logger;
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task<string> ReadBlobChunkAsync(string fileName, long startPoint, long endPoint)
        {
            try
            {
                string connectionString = _connectionString;
                string containerName = _containerName;

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(memoryStream);
                    string content = Encoding.UTF8.GetString(memoryStream.ToArray());
                    return content;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading blob chunk: {ex.Message}");
                throw;
            }
        }
    }
}
