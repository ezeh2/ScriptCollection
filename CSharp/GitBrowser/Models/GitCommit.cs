
// Models/GitCommit.cs
public class GitCommit
{
    public string Sha { get; set; }
    public string Message { get; set; }
    public string Author { get; set; }
    public DateTime AuthorDate { get; set; }
    public string Committer { get; set; }	
    public DateTime CommitterDate { get; set; }
    public List<string> Decorations { get; set; } = new List<string>();
    public List<string> Notes { get; set; } = new List<string>();
}
