using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System.Text;

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
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
                }

                if (startPoint < 0 || startPoint >= endPoint)
                {
                    throw new ArgumentOutOfRangeException(nameof(startPoint), "Invalid range specified. StartPoint must be less than EndPoint and non-negative.");
                }

                return await DownloadBlobChunkAsync(fileName, startPoint, endPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading blob chunk: {ex.Message}. FileName: {fileName}, StartPoint: {startPoint}, EndPoint: {endPoint}");
                throw;
            }
        }

        private async Task<string> DownloadBlobChunkAsync(string fileName, long startPoint, long endPoint)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            bool blobExists = await blobClient.ExistsAsync();
            if (!blobExists)
            {
                throw new FileNotFoundException($"The blob '{fileName}' does not exist in container '{_containerName}'.");
            }

            BlobProperties properties = await blobClient.GetPropertiesAsync();
            if (endPoint > properties.ContentLength)
            {
                throw new ArgumentOutOfRangeException(nameof(endPoint), "Endpoint is beyond the file boundary.");
            }

            var range = new Azure.HttpRange(startPoint, endPoint - startPoint + 1);

            var response = await blobClient.DownloadStreamingAsync(new BlobDownloadOptions { Range = range });

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await response.Value.Content.CopyToAsync(memoryStream);

                string result = Encoding.UTF8.GetString(memoryStream.ToArray());
                return result;
            }
        }
    }
}
