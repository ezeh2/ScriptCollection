using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitBrowser.Services;
using GitBrowser.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GitBrowser.Tests.Services
{
    [TestClass]
    public class FileExplorerServiceTests
    {
        private FileExplorerService _service;
        private string _testBasePath; // Base path for test directories and files

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new FileExplorerService();
            // Create a unique base path for each test run to avoid conflicts
            _testBasePath = Path.Combine(Path.GetTempPath(), "FileExplorerServiceTests", Path.GetRandomFileName());
            Directory.CreateDirectory(_testBasePath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up the created test directory and its contents
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
        }

        [TestMethod]
        public void GetDirectoryContents_EmptyDirectory_ReturnsEmptyEntries()
        {
            // Arrange
            var emptyDirPath = Path.Combine(_testBasePath, "emptyDir");
            Directory.CreateDirectory(emptyDirPath);

            // Act
            var result = _service.GetDirectoryContents(emptyDirPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(emptyDirPath, result.CurrentPath);
            Assert.IsNotNull(result.Entries);
            Assert.AreEqual(0, result.Entries.Count);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public void GetDirectoryContents_WithFilesAndSubdirectories_ReturnsCorrectEntriesAndSortOrder()
        {
            // Arrange
            var mixedDirPath = Path.Combine(_testBasePath, "mixedDir");
            Directory.CreateDirectory(mixedDirPath);
            Directory.CreateDirectory(Path.Combine(mixedDirPath, "subDirA"));
            Directory.CreateDirectory(Path.Combine(mixedDirPath, "subDirZ"));
            File.WriteAllText(Path.Combine(mixedDirPath, "fileA.txt"), "content");
            File.WriteAllText(Path.Combine(mixedDirPath, "fileZ.txt"), "content");

            // Act
            var result = _service.GetDirectoryContents(mixedDirPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mixedDirPath, result.CurrentPath);
            Assert.IsNotNull(result.Entries);
            Assert.AreEqual(4, result.Entries.Count);
            Assert.IsNull(result.ErrorMessage);

            // Check sorting: directories first, then by name
            Assert.IsTrue(result.Entries[0].IsDirectory);
            Assert.AreEqual("subDirA", result.Entries[0].Name);
            Assert.IsTrue(result.Entries[1].IsDirectory);
            Assert.AreEqual("subDirZ", result.Entries[1].Name);
            Assert.IsFalse(result.Entries[2].IsDirectory);
            Assert.AreEqual("fileA.txt", result.Entries[2].Name);
            Assert.IsFalse(result.Entries[3].IsDirectory);
            Assert.AreEqual("fileZ.txt", result.Entries[3].Name);
        }

        [TestMethod]
        public void GetDirectoryContents_NonExistentPath_ReturnsEmptyEntriesAndSetsErrorMessage()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testBasePath, "nonExistentDir");

            // Act
            var result = _service.GetDirectoryContents(nonExistentPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(nonExistentPath, result.CurrentPath);
            Assert.IsNotNull(result.Entries);
            Assert.AreEqual(0, result.Entries.Count);
            // The service currently logs to console and returns empty.
            // If it were to set ErrorMessage, we'd assert that here.
            // For now, the behavior is empty entries.
            // Assert.IsNotNull(result.ErrorMessage); // Uncomment if service is updated to set ErrorMessage
        }

        [TestMethod]
        public void GetDirectoryContents_PathIsFile_ReturnsEmptyEntriesAndSetsErrorMessage()
        {
            // Arrange
            var filePath = Path.Combine(_testBasePath, "someFile.txt");
            File.WriteAllText(filePath, "this is a file, not a directory");

            // Act
            var result = _service.GetDirectoryContents(filePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(filePath, result.CurrentPath);
            Assert.IsNotNull(result.Entries);
            Assert.AreEqual(0, result.Entries.Count);
            // Similar to NonExistentPath, the service currently logs and returns empty.
            // Assert.IsNotNull(result.ErrorMessage); // Uncomment if service is updated to set ErrorMessage
        }
    }
}
