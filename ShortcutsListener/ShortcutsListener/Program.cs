using System.Net;
using System.Net.Sockets;
using System.Text;
using TextCopy;
namespace ShortcutsListener
{
    class Program
    {
        private static byte[] responseBytes = Encoding.ASCII.GetBytes(HTTPRequest.BasicResponse);
        private static byte[] msg = new byte[10000];

        [STAThread]
        static void Main(string[] args)
        {
            // get folder from command line
            string downloadsFolder = string.Empty;

            if (OperatingSystem.IsWindows())
            {
                downloadsFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"Downloads\\");
            }
            else if (OperatingSystem.IsLinux())
            {
                downloadsFolder = Environment.GetEnvironmentVariable("HOME") + "/Downloads/";
            }
            var folder = (args.Length > 0) ? args[0] : downloadsFolder;
            // check if folder exists
            if (!Directory.Exists(folder)) {
                // enclose below line in try/catch block
                try
                {
                    Directory.CreateDirectory(folder);
                }
                catch (System.Exception)
                {
                    folder = "";    
                }

            }
            var port = 2560;
            Console.Write("Saving to folder : " + folder + "\nDifferent folder can be specified in arguments\n");
            // get current IP address
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            Console.WriteLine($"Current IP Address: {ipAddress}");
            var server = new TcpListener(IPAddress.Any, port);
            server.Start();

            while (true)
            {
                Console.WriteLine($"Listening on port {port}");
                var client = server.AcceptTcpClient();  //if a connection exists, the server will accept it
                var networkStream = client.GetStream(); //networkstream is used to send/receive messages

                HTTPRequest.Parse(networkStream);

                //get the file size 
                if (HTTPRequest.Headers.ContainsKey(HTTPReqHeaderKey.ContentLength) && int.TryParse(HTTPRequest.Headers[HTTPReqHeaderKey.ContentLength], out int numberOfbytesToRead))
                {
                    int readCounter = 0;
                    if (HTTPRequest.Headers.ContainsKey(HTTPReqHeaderKey.Clipboard)) { 
                        // variable to hold the text
                        var clipboardText = "";
                        while (readCounter < numberOfbytesToRead)
                        {
                            int numberOfBytesRead = networkStream.Read(msg, 0, msg.Length);
                            readCounter += numberOfBytesRead;
                            clipboardText += Encoding.UTF8.GetString(msg, 0, numberOfBytesRead);
                        }
                        Console.Write(clipboardText);
                        ClipboardService.SetText(clipboardText);
                        // Copy clipboardText to the clipboard
                        Console.WriteLine('\n');
                    } else {  // it's a file          
                        var fileName = HTTPRequest.Headers.ContainsKey(HTTPReqHeaderKey.FileName) 
                            ? HTTPRequest.Headers[HTTPReqHeaderKey.FileName] 
                            : $"file_{Guid.NewGuid()}";

                        //get the extention of the file its been always image/*, video/*, */*
                        var fileExtention = HTTPRequest.Headers[HTTPReqHeaderKey.ContentType].Split('/')[1].ToLower();


                        switch (fileExtention)
                        {
                            case "plain":
                                fileExtention = "txt";
                                break;
                            case "quicktime": //ios puts video/quicktime content-type header for their video files.
                                fileExtention = "mov";
                                break;
                            case "javascript":
                                fileExtention = "js";
                                break;
                            case "mpeg":
                                fileExtention = "mp3";
                                break;
                            case "svg+xml":
                                fileExtention = "svg";
                                break;
                            case "wave":
                                fileExtention = "wav";
                                break;
                            case "x-www-form-urlencoded":
                                fileExtention = "txt";
                                break;
                            default:
                                break;
                        }

                        fileName = fileName + "." + fileExtention;

                        Console.WriteLine(fileName);
                        var fileStream = new FileStream(folder + fileName, FileMode.Create);

                        while (readCounter < numberOfbytesToRead)
                        {
                            int numberOfBytesRead = networkStream.Read(msg, 0, msg.Length);
                            readCounter += numberOfBytesRead;
                            fileStream.Write(msg, 0, numberOfBytesRead);
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($"{((UInt64)readCounter * 100) / ((UInt64)numberOfbytesToRead - 1)}% Completed");
                        }
                        fileStream.Close();
                        Console.WriteLine('\n');
                    }
                }
                networkStream.Write(responseBytes, 0, responseBytes.Length);
                networkStream.Close();
                client.Close();
            }
        }
    }
}
