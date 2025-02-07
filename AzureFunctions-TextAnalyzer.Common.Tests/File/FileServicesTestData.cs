namespace AzureFunctions_TextAnalyzer.Common.Tests.Data
{
    public static class FileServicesTestData
    {
        // Invalid test cases for ReadBlobChunkAsync
        public static IEnumerable<object[]> BlobChunkErrorData()
        {
            return new List<object[]>
            {
                new object[] { "testfile.txt", 10L, 5L, typeof(ArgumentOutOfRangeException) },
                new object[] { "testfile.txt", -1L, 10L, typeof(ArgumentOutOfRangeException) },
                new object[] { "nonexistentfile.txt", 0L, 10L, typeof(FileNotFoundException) },
                new object[] { "", 0L, 10L, typeof(ArgumentNullException) },
                new object[] { null, 0L, 10L, typeof(ArgumentNullException) },
            };
        }

        // Valid test cases for ReadBlobChunkAsync
        public static IEnumerable<object[]> ReadBlobChunkSucessfulData()
        {
            return new List<object[]>
            {
                new object[] { "testfile1.txt", "This is a test file content.", 0, 9, "This is a " },
                new object[] { "testfile2.txt", "Another test content.", 0, 6, "Another" },
                new object[] { "testfile3.txt", "Blob Storage Test Data", 5, 11, "Storage" },
                new object[] { "testfile4.txt", "Short content", 0, 6, "Short c" },
            };
        }
    }
}
