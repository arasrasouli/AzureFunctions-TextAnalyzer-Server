using AzureFunctions_TextAnalyzer.Common.Helper;

namespace AzureFunctions_TextAnalyzer.Common.Tests
{
    public class StringHelperTests
    {
        [Theory]
        [InlineData("Hello World", 5)] 
        [InlineData("Hello World Again", 11)]
        [InlineData(" Hello World Again", 12)]
        [InlineData("NoSpacesHere", 0)]
        [InlineData("  LeadingSpaces", 1)]
        [InlineData("TrailingSpaces ", 14)]
        [InlineData("", 0)]
        [InlineData(" ", 0)]
        [InlineData("  ", 1)]
        public void FindIndexOfLastSpace_ShouldReturnCorrectIndex(string input, int expected)
        {
            // Act
            int result = StringHelper.FindIndexOfLastSpace(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Hello@World!", "Hello World")]
        [InlineData("Hello  World!!", "Hello World")]
        [InlineData("Hello_World", "Hello World")]
        [InlineData("Hello123World", "Hello123World")]
        [InlineData("   Hello   World   ", "Hello World")]
        [InlineData("1234!@#$%^&*", "1234")]
        [InlineData("  ", "")]
        public void ReplaceNonWordWithSpace_ShouldReturnExpectedString(string input, string expected)
        {
            // Act
            string result = StringHelper.ReplaceNonWordWithSpace(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReplaceNonWordWithSpace_ShouldThrowArgumentNullException_WhenTextIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => StringHelper.ReplaceNonWordWithSpace(null));
        }

        [Theory]
        [InlineData("My File", 3, "My_File_Chunks_3")]
        [InlineData("FileName", 5, "FileName_Chunks_5")]
        [InlineData("File@Name!", 7, "File@Name!_Chunks_7")]
        [InlineData("MyFile", 0, "MyFile_Chunks_0")]
        [InlineData("MyFile", -1, "MyFile_Chunks_-1")]
        public void GeneratePartitionKey_ShouldReturnExpectedResult(string fileName, int chunkCount, string expected)
        {
            // Act
            string result = StringHelper.GeneratePartitionKey(fileName, chunkCount);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public void GeneratePartitionKey_ShouldThrowArgumentException_ForInvalidFileName(string fileName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => StringHelper.GeneratePartitionKey(fileName, 3));
        }
    }
}
