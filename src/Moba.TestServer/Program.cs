
using NetworkLayer;


int port = 5000;
Guid matchId = Guid.Empty;

for (int i = 0; i < args.Length; i++)
{
    
    if (args[i] == "--port" && i + 1 < args.Length)
    {
        if (int.TryParse(args[i + 1], out int parsedPort))
        {
            port = parsedPort;
            i++;
        }
    }
    else if (args[i] == "--match" && i + 1 < args.Length)
    {
        if (Guid.TryParse(args[i + 1], out Guid matchParsedId))
        {
            matchId = matchParsedId;
            i++;
        }

    }

}

Console.Title = $"Server Match: {matchId} [Port:{port}]";
Console.WriteLine($"============================================");
Console.WriteLine($"[Server] Match ID : {matchId}");
 Console.WriteLine($"[Server] Binding Port: {port}");
Console.WriteLine($"============================================\n");


NetworkManager networkManager = new NetworkManager(port);
networkManager.Start();

Console.WriteLine($"============================= TEST SERVER STARTED =======================");

Thread.Sleep(Timeout.Infinite);