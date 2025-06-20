using System.Collections.Generic;

public class FileExplorerViewModel
{
	public string CurrentPath { get; set; }
	public List<FileSystemEntry> Entries { get; set; }
	public string ErrorMessage { get; set; } // Optional: For displaying errors to the user

	public FileExplorerViewModel()
	{
		Entries = new List<FileSystemEntry>();
	}
}

public class FileSystemEntry
{
	public string Name { get; set; }
	public string FullPath { get; set; }
	public bool IsDirectory { get; set; }
}


