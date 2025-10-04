# Other Installation Issues Encountered

1. **Title of Issue:** `codeql` command unavailable after downloading archive
   * **Reason it does not work:** After downloading the CodeQL CLI release archive there is no executable on the `PATH`, so invoking `codeql` results in `bash: command not found: codeql`.
   * **How to make it work:** Extract the archive into a permanent directory (for example `/opt/codeql`) and update the `PATH` to include the `codeql` executable, e.g. by adding `export PATH="/opt/codeql:$PATH"` to the shell profile.

2. **Title of Issue:** Missing execution permissions on the `codeql` binary after extraction
   * **Reason it does not work:** Depending on how the archive is extracted, the CLI binary may not retain executable permissions, preventing it from running even when located on the `PATH`.
   * **How to make it work:** Ensure the binary is executable by running `chmod +x /opt/codeql/codeql` (adjusting the path as needed) before attempting to invoke the CLI.
