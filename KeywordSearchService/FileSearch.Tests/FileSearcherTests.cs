using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using FileSearch;

namespace FileSearch.Tests
{
    public class FileSearcherTests
    {
        private readonly Mock<IFileSearcher> _fileSearcherMock;

        public FileSearcherTests()
        {
            _fileSearcherMock = new Mock<IFileSearcher>();
        }

        [Fact]
        public void SearchFiles_ShouldReturnResults_WhenKeywordIsFound()
        {
            // Arrange
            string testDirectory = "TestDirectory";
            string testKeyword = "keyword";
            List<string> expectedFiles = new List<string> { "file1.txt", "file2.txt" };

            _fileSearcherMock.Setup(fs => fs.SearchFiles(testDirectory, testKeyword))
                             .Returns(expectedFiles);

            // Act
            var results = _fileSearcherMock.Object.SearchFiles(testDirectory, testKeyword);

            // Assert
            Assert.Equal(expectedFiles.Count, results.Count);
            Assert.Equal(expectedFiles, results);
        }

        [Fact]
        public void SearchFiles_ShouldReturnEmptyList_WhenKeywordIsNotFound()
        {
            // Arrange
            string testDirectory = "TestDirectory";
            string testKeyword = "nonexistent_keyword";

            _fileSearcherMock.Setup(fs => fs.SearchFiles(testDirectory, testKeyword))
                             .Returns(new List<string>());

            // Act
            var results = _fileSearcherMock.Object.SearchFiles(testDirectory, testKeyword);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void SearchFiles_ShouldSkipFiles_WhenAccessDenied()
        {
            // Arrange
            string testDirectory = "TestDirectory";
            string testKeyword = "keyword";
            var fileSearcher = new FileSearcher();

            Directory.CreateDirectory(testDirectory);
            string filePath = Path.Combine(testDirectory, "tempFile.txt");

            try
            {
                File.WriteAllText(filePath, "This is a test file with the keyword.");

                // Opens file to block the access
                using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // Act
                    var results = fileSearcher.SearchFiles(testDirectory, testKeyword);

                    // Assert
                    Assert.DoesNotContain(filePath, results);
                }
            }
            finally
            {
                Directory.Delete(testDirectory, true);
            }
        }
    }
}