using AzureFunctions_TextAnalyzer.Common;
using AzureFunctions_TextAnalyzer.Common.Tests;
using AzureFunctions_TextAnalyzer.Common.Tests.Data;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;

namespace AzureFunctions_TextAnalyzer.Tests
{
    [Collection("BlobContainerCollection")]
    public class FileServicesTests
    {
        private readonly BlobContainerFixture _fixture;

        public FileServicesTests(BlobContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(FileServicesTestData.ReadBlobChunkSucessfulData), MemberType = typeof(FileServicesTestData))]
        public async Task ReadBlobChunkAsync_ShouldReturnCorrectContent(string fileName, string fileContent, long startPoint, long endPoint, string expectedChunk)
        {
            // Arrange
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            await using var fileStream = new MemoryStream(fileBytes);

            var mockLogger = Substitute.For<ILogger<FileServices>>();

            var blobClient = _fixture.ContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);

            var fileServices = new FileServices(mockLogger, _fixture.ConnectionString, _fixture.ContainerName);

            // Act
            string actualContent = await fileServices.ReadBlobChunkAsync(fileName, startPoint, endPoint);

            // Assert
            Assert.Equal(expectedChunk, actualContent);
        }

        [Theory]
        [MemberData(nameof(FileServicesTestData.BlobChunkErrorData), MemberType = typeof(FileServicesTestData))]
        public async Task ReadBlobChunkAsync_ShouldThrowException(string fileName, long startPoint, long endPoint, Type expectedExceptionType)
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<FileServices>>();
            var fileServices = new FileServices(mockLogger, _fixture.ConnectionString, _fixture.ContainerName);

            // Act & Assert
            await Assert.ThrowsAsync(expectedExceptionType, async () => await fileServices.ReadBlobChunkAsync(fileName, startPoint, endPoint));
        }

        [Fact]
        public async Task ReadBlobChunkAsync_EndpointGreaterThanLength_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string fileName = "testfile.txt";
            string fileContent = "This is a test file.";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            await using var fileStream = new MemoryStream(fileBytes);

            var mockLogger = Substitute.For<ILogger<FileServices>>();
            var blobClient = _fixture.ContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);

            var fileServices = new FileServices(mockLogger, _fixture.ConnectionString, _fixture.ContainerName);

            long fileLength = fileContent.Length;
            long startPoint = 0;
            long endPoint = fileLength + 10;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await fileServices.ReadBlobChunkAsync(fileName, startPoint, endPoint));
        }
    }
}
