namespace GitBrowser.Models
{
    public class GitCommitChange
    {
        public string FileName { get; set; }
        public string Status { get; set; } // e.g., "Added", "Modified", "Deleted", "Renamed", "Copied"

        // Consider adding properties for lines added/deleted if needed later
        // public int LinesAdded { get; set; }
        // public int LinesDeleted { get; set; }
    }
}
