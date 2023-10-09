using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace InterServer;

/*
__        ___    ____  _   _ ___ _   _  ____
\ \      / / \  |  _ \| \ | |_ _| \ | |/ ___|
 \ \ /\ / / _ \ | |_) |  \| || ||  \| | |  _
  \ V  V / ___ \|  _ <| |\  || || |\  | |_| |
   \_/\_/_/   \_\_| \_\_| \_|___|_| \_|\____|
   
   Code below is EXPERIMENTAL.
*/

class Program
{
    static void Main()
    {
        // Config
        string listenTarget = "http://localhost:8080/";
        /*
         * Windows fucking sucks for hosting.
         * This can't be done in non-admin mode, and thus requires additional logic.
         * It must be either prompted to run in admin mode, or default to localhost on Windows.
         * FML, I expected it to work at least!
         */
        // string listenTarget = $"http://{GetLocalIPAddress()}:8080/";
        
        
        
        // Create a new HttpListener instance
        var listener = new HttpListener();
        
        // Add prefixes to specify the URL(s) to listen on
        listener.Prefixes.Add(listenTarget); // Change the URL as needed
        
        Console.WriteLine($"Listening for requests on {listenTarget}...");

        // Start listening for incoming requests
        listener.Start();

        // Create a cancellation token source for graceful shutdown
        var cancellationTokenSource = new CancellationTokenSource();

        // Handle incoming requests on a separate thread
        ThreadPool.QueueUserWorkItem(state =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // Wait for an incoming request
                    var context = listener.GetContext();

                    // Handle the request on a separate thread
                    ThreadPool.QueueUserWorkItem(requestState =>
                    {
                        var httpListenerContext = (HttpListenerContext)requestState;
                        var request = httpListenerContext.Request;
                        var response = httpListenerContext.Response;

                        // Log the request
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - Received request: {request.HttpMethod} {request.Url}");
                        
                        // Whole further processing logic
                        // Look into RequestHandler.cs for more info
                        RequestHandler handler = new RequestHandler(request, response);
                    }, context);
                }
                catch (HttpListenerException e)
                {
                    // Handle exceptions as needed
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} - ERROR in HTTP listener!\n{e.Message}");
                }
            }

            // Stop the listener when the cancellation token is triggered
            listener.Stop();
            listener.Close();
        }, null);

        // Press Enter to exit
        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        // Signal cancellation to stop the listener gracefully
        cancellationTokenSource.Cancel();
        Console.WriteLine("Listener stopped.");
    }
    
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}