# GitBrowser

GitBrowser is a web application that allows users to browse Git repositories on the local file system. It can list repositories, branches, and commits.

## Implementation

GitBrowser is an ASP.NET Core application written in C#.
It uses the LibGit2Sharp library to interact with Git repositories.
The application follows the Model-View-Controller (MVC) pattern.
- The `GitController` handles incoming requests and uses the `GitService` to fetch data from Git repositories.
- The `GitService` uses LibGit2Sharp to interact with the Git repositories.
- The views are Razor views that display the data fetched by the controller.
