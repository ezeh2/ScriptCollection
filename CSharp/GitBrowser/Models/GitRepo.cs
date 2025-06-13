// Models/GitRepo.cs
public class GitRepo
{
    public string Name { get; set; }
    public string Path { get; set; }
}

// Models/GitBranch.cs
public class GitBranch
{
    public string Name { get; set; }
}

// Models/GitCommit.cs
public class GitCommit
{
    public string Sha { get; set; }
    public string Message { get; set; }
    public string Author { get; set; }
    public DateTime AuthorDate { get; set; }
}
