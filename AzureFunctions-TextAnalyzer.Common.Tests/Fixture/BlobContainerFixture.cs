using Azure.Storage.Blobs;

namespace AzureFunctions_TextAnalyzer.Common.Tests
{
    public class BlobContainerFixture : IAsyncLifetime
    {
        public readonly string ConnectionString = "UseDevelopmentStorage=true";
        public readonly string ContainerName = $"test-container-{Guid.NewGuid()}";
        public BlobContainerClient ContainerClient { get; private set; }

        public async Task InitializeAsync()
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            ContainerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            await ContainerClient.CreateIfNotExistsAsync();
        }

        public async Task DisposeAsync()
        {
            if (ContainerClient != null)
            {
                await ContainerClient.DeleteIfExistsAsync();
            }
        }
    }


    [CollectionDefinition("BlobContainerCollection")]
    public class BlobContainerCollection : ICollectionFixture<BlobContainerFixture>
    {
    }
}
