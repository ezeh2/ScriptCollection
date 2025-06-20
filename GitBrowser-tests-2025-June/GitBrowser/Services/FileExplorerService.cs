using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBrowser.Services
{
    public class FileExplorerService : IFileExplorerService
    {
		public FileExplorerService()
		{
		}
		
        public FileExplorerViewModel GetDirectoryContents(string path)
        {
            var viewModel = new FileExplorerViewModel
            {
                CurrentPath = path,
                Entries = new List<FileSystemEntry>()
            };

            try
            {
                // Get all files and directories
                var fileSystemEntries = Directory.GetFileSystemEntries(path);

                foreach (var entryPath in fileSystemEntries)
                {
                    var entryName = Path.GetFileName(entryPath);
                    var isDirectory = Directory.Exists(entryPath); // Or File.GetAttributes(entryPath).HasFlag(FileAttributes.Directory);

                    viewModel.Entries.Add(new FileSystemEntry
                    {
                        Name = entryName,
                        FullPath = entryPath, // Storing full path for easier linking later
                        IsDirectory = isDirectory
                    });
                }

                // Optional: Sort entries, e.g., directories first, then files, then by name
                viewModel.Entries = viewModel.Entries
                    .OrderByDescending(e => e.IsDirectory)
                    .ThenBy(e => e.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., path not found, access denied)
                // For now, we can let it propagate or log it.
                // Or add an error message to the view model.
                // For simplicity, if an error occurs, we'll return an empty list of entries.
                // A more robust solution would involve logging and user feedback.
                Console.WriteLine($"Error accessing path {path}: {ex.Message}");
                viewModel.Entries = new List<FileSystemEntry>(); // Clear entries on error
                // Optionally, set an error message on the viewModel
                // viewModel.ErrorMessage = $"Error accessing path: {ex.Message}";
            }

            return viewModel;
        }
    }
}
