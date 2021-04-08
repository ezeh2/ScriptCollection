
# This powershell script opens fileexplorer in specified path.
# If there is already a File Explorer open, then the path in that window is changed.

Param (
  $path
)

# timestamp
$dt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# write path to logfile
Add-Content -Path "WindowsExplorerChangeLocationPaths.txt" -Value $dt" "$path

# win32-function related to windows:
#  [WinAp]::ShowWindow(...)
#  [WinAp]::SetForegroundWindow(...)
#  https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
<#
SW_RESTORE:
Activates and displays the window. If the window is minimized or maximized, the system restores 
it to its original size and position. An application should specify this flag when restoring a minimized window.
#>
$SW_RESTORE=9
Add-Type @"
    using System;
    using System.Runtime.InteropServices;
    public class WinAp {
      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool SetForegroundWindow(IntPtr hWnd);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
"@

$shell_application=new-object -com Shell.Application
$windows = $shell_application.Application.Windows()

# is there at least one window ?
if ($windows.Count -eq 0) {
  # no, open a new window
  $shell_application.Application.Explore($path)
}
else {

  $location = $windows[0].LocationURL
  Add-Content -Path "WindowsExplorerChangeLocationPaths.txt" -Value $dt" "$location

  # take first window and naviaget to path
  $windows[0].Navigate($path)
  # if window is minimized
  [WinAp]::ShowWindow($windows[0].HWND,$SW_RESTORE)
  # if window is in background
  [WinAp]::SetForegroundWindow($windows[0].HWND)
}



