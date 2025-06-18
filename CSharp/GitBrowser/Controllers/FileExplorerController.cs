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

            var directoryContentsViewModel = _fileExplorerService.GetDirectoryContents(path);
            // The service now sets ParentPath internally

            ViewBag.CurrentPath = path; // Can remain, or be removed if view fully relies on Model.
            return View("FileExplorer", directoryContentsViewModel);
        }
    }
}
