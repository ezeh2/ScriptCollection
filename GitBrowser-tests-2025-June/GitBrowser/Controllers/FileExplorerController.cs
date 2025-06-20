using Microsoft.AspNetCore.Mvc;
using GitBrowser.Services; // Assuming FileExplorerService is in this namespace
using System.Collections.Generic;

namespace GitBrowser.Controllers
{
    public class FileExplorerController : Controller
    {
        private readonly IFileExplorerService _fileExplorerService;

        public FileExplorerController(IFileExplorerService fileExplorerService)
        {
            _fileExplorerService = fileExplorerService;
        }

        public IActionResult Index(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "/"; // Default to root directory if no path is provided
            }

            // Assuming FileExplorerService has a method GetDirectoryContents
            // that returns a list of objects representing files and directories.
            // Let's define a simple model for this, e.g., FileEntryViewModel.
            var directoryContents = _fileExplorerService.GetDirectoryContents(path);

            ViewBag.CurrentPath = path;
            return View("FileExplorer", directoryContents);
        }
    }
}
