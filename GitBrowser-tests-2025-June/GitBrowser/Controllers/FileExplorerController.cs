using Microsoft.AspNetCore.Mvc;
using GitBrowser.Services; // Assuming FileExplorerService is in this namespace
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBrowser.Controllers
{
    public class FileExplorerController : Controller
    {
        private readonly IFileExplorerService _fileExplorerService;
        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars().Concat(new char[] { '?', '*', ':', '<', '>', '|' }).Distinct().ToArray();

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
            else
            {
                // Validate path for invalid characters
                if (path.Any(c => InvalidPathChars.Contains(c)))
                {
                    return BadRequest("The path contains invalid characters.");
                }
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
