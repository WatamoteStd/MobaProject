
using System.Net.Http.Headers;
using System.Threading.Channels;
using NetworkLayer;
using NetworkLayer.DataStructs;
using Phases;


Channel<NetworkCommand> channel = Channel.CreateUnbounded<NetworkCommand>();

int port = 5000;
Guid matchId = Guid.Empty;
int playersCount = 0;

HashSet<long> allowedPlayers = new HashSet<long>();

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
    else if (args[i] == "--players" && i + 1 < args.Length)
    {
        if(int.TryParse(args[i + 1], out int count))
        {
            playersCount = count;
        }
    }
    else if (args[i] == "--playersIds" && i + 1 < args.Length)
    {
        
        var allIds = args[i + 1];
        string[] splitedIds = allIds.Split(',');

        for(int j = 0; j < splitedIds.Length; j++)
        {
            if(long.TryParse(splitedIds[j], out var pId))
            {
                allowedPlayers.Add(pId);
            }
        }

    }

}


Console.Title = $"Server Match: {matchId} [Port:{port}]";
Console.WriteLine($"============================================");
Console.WriteLine($"[Server] Match ID : {matchId}");
Console.WriteLine($"[Server] Binding Port: {port}");
Console.WriteLine($"============================================\n");


NetworkManager networkManager = new NetworkManager(port, channel.Writer);
networkManager.Start();

PhaseManager _phaseManager = new PhaseManager(playersCount, channel.Reader, matchId, allowedPlayers);

Console.WriteLine($"============================= TEST SERVER STARTED =======================");

_phaseManager.StartMatch();

Thread.Sleep(Timeout.Infinite);