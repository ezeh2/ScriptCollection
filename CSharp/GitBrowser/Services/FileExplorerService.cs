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
            };

            viewModel.ParentPath = CalculateParentPathLogic(viewModel.CurrentPath);

            try
            {
                // Get all files and directories
                var fileSystemEntries = Directory.GetFileSystemEntries(path);

                foreach (var entryPath in fileSystemEntries)
                {
                    var entryName = Path.GetFileName(entryPath);
                    var isDirectory = Directory.Exists(entryPath); // Or File.GetAttributes(entryPath).HasFlag(FileAttributes.Directory);

                    if (isDirectory)
                    {
                        viewModel.DirectoryEntries.Add(new FileSystemEntry
                        {
                            Name = entryName,
                            FullPath = entryPath,
                            IsDirectory = true
                        });
                    }
                    else
                    {
                        viewModel.FileEntries.Add(new FileSystemEntry
                        {
                            Name = entryName,
                            FullPath = entryPath,
                            IsDirectory = false
                        });
                    }
                }

                viewModel.DirectoryEntries = viewModel.DirectoryEntries
                                                .OrderBy(e => e.Name)
                                                .ToList();
                viewModel.FileEntries = viewModel.FileEntries
                                            .OrderBy(e => e.Name)
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
                viewModel.DirectoryEntries = new List<FileSystemEntry>();
                viewModel.FileEntries = new List<FileSystemEntry>();
                viewModel.ErrorMessage = $"Error accessing path: {ex.Message}";
            }

            return viewModel;
        }

        public static string CalculateParentPathLogic(string currentPath)
        {
            string calculatedParentPath = null;
            if (currentPath != null && currentPath.TrimEnd('/').Length > 0 && currentPath != "/")
            {
                calculatedParentPath = System.IO.Path.GetDirectoryName(currentPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));

                if (string.IsNullOrEmpty(calculatedParentPath))
                {
                    var pathRoot = System.IO.Path.GetPathRoot(currentPath);
                    if (!string.IsNullOrEmpty(pathRoot) && currentPath != pathRoot)
                    {
                        calculatedParentPath = pathRoot;
                    }
                    else if (currentPath.Contains(System.IO.Path.DirectorySeparatorChar) || currentPath.Contains(System.IO.Path.AltDirectorySeparatorChar))
                    {
                        if (currentPath != "/") calculatedParentPath = "/";
                        else calculatedParentPath = null;
                    }
                    else
                    {
                        calculatedParentPath = "/";
                    }
                }
            }

            if (calculatedParentPath != null &&
                calculatedParentPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) ==
                currentPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar))
            {
                // This condition handles when current path is a root like "C:\" and GetDirectoryName returns "C:\"
                // OR when currentPath is "folder/" and calculatedParentPath becomes "folder"
                // We need to ensure it correctly nullifies for true roots.
                if (currentPath == System.IO.Path.GetPathRoot(currentPath) && !string.IsNullOrEmpty(System.IO.Path.GetPathRoot(currentPath))) {
                    calculatedParentPath = null;
                }
                // Special handling if path was like "C:/folder" and parent became "C:/" (not null)
                // but if path was "C:/" and parent became "C:/", it SHOULD be null.
                // The original logic had an explicit check for currentPath == "/" AFTER this block.
            }

            // If currentPath is exactly "/", parentPath must be null. This overrides previous logic if necessary.
            if (currentPath == "/") {
                calculatedParentPath = null;
            }
            // One final check: if calculatedParentPath ended up same as currentPath (and not root like "/"), it means it's likely a root drive like C:        // and GetDirectoryName returned C:\. In this case, parent should be null.
            // This is a bit redundant with the GetPathRoot check above but acts as a safeguard.
            else if (calculatedParentPath != null &&
                     calculatedParentPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) ==
                     currentPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) &&
                     currentPath != "/") // ensure not for "/"
            {
                calculatedParentPath = null;
            }

            return calculatedParentPath;
        }
    }
}
