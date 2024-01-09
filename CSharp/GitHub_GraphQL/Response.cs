using System;
using System.Collections.Generic;

public class Cvss
{
    public double Score { get; set; }
    public string? VectorString { get; set; }
}

public class Advisory
{
    public string? Classification { get; set; }
    public string? Summary { get; set; }
    public string? GhsaId { get; set; }
    public Cvss? Cvss { get; set; }
}

public class Package
{
    public string? Name { get; set; }
    public string? Ecosystem { get; set; }
}

public class Node
{
    public Package? Package { get; set; }
    public string? VulnerableVersionRange { get; set; }
    public string? Severity { get; set; }
    public Advisory? Advisory { get; set; }
}

public class Edge
{
    public string? Cursor { get; set; }
    public Node? Node { get; set; }
}

public class SecurityVulnerabilities
{
    public List<Edge>? Edges { get; set; }
}

public class Data
{
    public SecurityVulnerabilities? SecurityVulnerabilities { get; set; }
}

public class Root
{
    public Data? Data { get; set; }
}
