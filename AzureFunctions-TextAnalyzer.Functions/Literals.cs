namespace AzureFunctions_TextAnalyzer.Functions
{
    public static class Literals
    {
        public const string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
        public const string ServiceBusConnectionString = nameof(ServiceBusConnectionString);
        public const string BlobContainer = nameof(BlobContainer);
        public const string ChunkSize = nameof(ChunkSize);
        public const string OverlapSize = nameof(OverlapSize);
        public const string TableStorageConnectionString = nameof(TableStorageConnectionString);
        public const string ChunkTableName = nameof(ChunkTableName);
        public const string FileTableName = nameof(FileTableName);
    }
}
