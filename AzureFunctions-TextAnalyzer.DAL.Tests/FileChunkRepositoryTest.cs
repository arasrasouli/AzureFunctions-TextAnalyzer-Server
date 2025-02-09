using Azure;
using Azure.Data.Tables;
using AzureFunctions_TextAnalyzer.DAL.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using System.Net;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories.Tests
{
    public class FileChunkRepositoryTests
    {
        private readonly TableClient _mockTableClient;
        private readonly FileChunkRepository _repository;

        public FileChunkRepositoryTests()
        {
            _mockTableClient = Substitute.For<TableClient>();
            _repository = new FileChunkRepository(_mockTableClient);
        }

        [Fact]
        public async Task UpdateEntityAsync_Success_DecreasesRemainingChunks()
        {
            // Arrange
            string partitionKey = "testPartition";
            string rowKey = "testRow";

            FileChunkEntity entity = new FileChunkEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                RemainingChunks = 10,
                ETag = ETag.All
            };

            FileChunkEntity updatedEntity = new FileChunkEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                RemainingChunks = 9,
                ETag = ETag.All
            };

            Response<FileChunkEntity> mockGetResponse = Substitute.For<Response<FileChunkEntity>>();
            mockGetResponse.Value.Returns(entity);

            Response mockUpdateResponse = Substitute.For<Response>();

            _mockTableClient
                .GetEntityAsync<FileChunkEntity>(partitionKey, rowKey, null, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(mockGetResponse));

            _mockTableClient
                .UpdateEntityAsync(Arg.Any<FileChunkEntity>(), Arg.Any<ETag>(), TableUpdateMode.Replace, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(mockUpdateResponse));

            // Act
            FileChunkEntity result = await _repository.UpdateRemainingChunksAsync(partitionKey, rowKey, 3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(9, result.RemainingChunks);
            await _mockTableClient.Received(1).GetEntityAsync<FileChunkEntity>(
                partitionKey,
                rowKey,
                null,
                Arg.Any<CancellationToken>()
            );
        }

        [Fact]
        public async Task UpdateEntityAsync_CallsMultipleTimes_DecreasesToZero()
        {
            // Arrange
            string partitionKey = "testPartition";
            string rowKey = "testRow";

            FileChunkEntity entity = new FileChunkEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                RemainingChunks = 10,
                ETag = ETag.All
            };

            var mockGetResponse = Substitute.For<Response<FileChunkEntity>>();
            mockGetResponse.Value.Returns(entity);

            Response mockUpdateResponse = Substitute.For<Response>();

            _mockTableClient
                .GetEntityAsync<FileChunkEntity>(partitionKey, rowKey, null, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(mockGetResponse));

            _mockTableClient
                .UpdateEntityAsync(Arg.Any<FileChunkEntity>(), Arg.Any<ETag>(), TableUpdateMode.Replace, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(mockUpdateResponse));

            // Act
            for (int i = 0; i < 10; i++)
            {
                var result = await _repository.UpdateRemainingChunksAsync(partitionKey, rowKey, 3);

                mockGetResponse.Value.Returns(entity);
            }

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(0, entity.RemainingChunks);

            await _mockTableClient.Received(10).GetEntityAsync<FileChunkEntity>(
                partitionKey,
                rowKey,
                null,
                Arg.Any<CancellationToken>()
            );
        }

        [Fact]
        public async Task UpdateEntityAsync_HandlesRaceCondition_WithRetriesOnGetEntity()
        {
            // Arrange
            string partitionKey = "testPartition";
            string rowKey = "testRow";
            int maxRetries = 2;

            FileChunkEntity entity = new FileChunkEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                RemainingChunks = 10,
                ETag = ETag.All
            };

            // Mocked responses
            Response<FileChunkEntity> mockGetResponse = Substitute.For<Response<FileChunkEntity>>();
            mockGetResponse.Value.Returns(entity);

            Response mockUpdateResponse = Substitute.For<Response>();

            int getEntityCallCount = 0;

            _mockTableClient
                .GetEntityAsync<FileChunkEntity>(partitionKey, rowKey, null, Arg.Any<CancellationToken>())
                .Returns(async _ =>
                {
                    getEntityCallCount++;

                    if (entity.ETag == ETag.All && getEntityCallCount <= 2)
                    {
                        entity.ETag = new ETag($"etag-{getEntityCallCount}");
                        throw new RequestFailedException(412, "Concurrency conflict on GetEntity");
                    }

                    return mockGetResponse;
                });


            _mockTableClient
                .UpdateEntityAsync(Arg.Any<FileChunkEntity>(), Arg.Any<ETag>())
                .Returns(async _ =>
                {
                    return mockUpdateResponse;
                });

            Task task1 = Task.Run(async () => await _repository.UpdateRemainingChunksAsync(partitionKey, rowKey, maxRetries));
            Task task2 = Task.Run(async () =>
            {
                await Task.Delay(50);
                return await _repository.UpdateRemainingChunksAsync(partitionKey, rowKey, maxRetries);
            });

            // Act
            await Task.WhenAll(task1, task2);

            // Assert
            Assert.Equal(8, entity.RemainingChunks);

            Assert.InRange(getEntityCallCount, 3, 4);

            await _mockTableClient.Received(getEntityCallCount).GetEntityAsync<FileChunkEntity>(
                partitionKey,
                rowKey,
                null,
                Arg.Any<CancellationToken>()
            );

            await _mockTableClient.Received(2).UpdateEntityAsync(
                Arg.Is<FileChunkEntity>(e => e.PartitionKey == partitionKey && e.RowKey == rowKey),
                Arg.Any<ETag>()
            );
        }

        [Fact]
        public async Task UpdateEntityAsync_ThrowsException_OnConflict()
        {
            // Arrange
            string partitionKey = "testPartition";
            string rowKey = "testRow";

            FileChunkEntity entity = new FileChunkEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                RemainingChunks = 10,
                ETag = ETag.All
            };

            RequestFailedException requestFailedException = new RequestFailedException((int)HttpStatusCode.PreconditionFailed, "Conflict occurred");
            _mockTableClient
                .UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace, Arg.Any<CancellationToken>())
                .Throws(requestFailedException);

            // Act & Assert
            RequestFailedException exception = await Assert.ThrowsAsync<RequestFailedException>(() =>
                _mockTableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace)
            );

            Assert.Equal((int)HttpStatusCode.PreconditionFailed, exception.Status);
            await _mockTableClient.Received(1).UpdateEntityAsync(
                Arg.Is<FileChunkEntity>(e => e.PartitionKey == partitionKey && e.RowKey == rowKey),
                entity.ETag,
                TableUpdateMode.Replace
            );
        }
    }
}