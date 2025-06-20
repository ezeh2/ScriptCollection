
namespace GitBrowser.Services
{
    public interface IFileExplorerService
    {
        FileExplorerViewModel GetDirectoryContents(string path);
    }
}
