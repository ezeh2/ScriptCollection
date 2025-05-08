# What is xit?
xit is a minimal Git-based tracking tool for individual files on a system. It wraps around a bare Git repository located at /home/xit and treats the root (/) as the working directory.

Its purpose is to track and version individual files—like system configuration files—without touching or committing everything on the system.

# How It Works
A bare Git repository is created at /home/xit if it doesn't already exist.

A global .gitignore in / ignores all files (*) by default.

When a file is tracked, xit automatically un-ignores it and commits it.

The working directory is always /.

# Available Commands
## 1. xit track <file> <message>
Tracks a single file.

Updates .gitignore to allow Git to see that file.

Adds both the file and the .gitignore to the repo.

Commits the change with the given message.

Example:

bash
Copy
Edit
xit track /etc/hosts "Start tracking hosts file"
Restrictions:

You cannot track a directory or use xit track ..

Only one file may be tracked per command.

## 2. xit status
Shows the current Git status of the tracked files.

## 3. xit log
Shows a compact commit history of tracked changes, with a visual graph.

This makes xit useful for system administrators, power users, or developers who want lightweight version control over key files (e.g., /etc/hosts, /etc/fstab, etc.) without cluttering or touching unrelated files.