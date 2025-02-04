namespace AzureFunctions_TextAnalyzer.Common.Enums
{
    public enum FileProcessingStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3,
        Queued = 4
    }
}