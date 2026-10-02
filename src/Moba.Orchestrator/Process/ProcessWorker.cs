
using System.Threading.Channels;
using Core;
using Moba.Shared.OrchestratorData;

namespace Process;

public class ProcessWorker
{
    private readonly PortManager _portManager;
    private readonly ChannelReader<MatchCreateInfo> _reader;
    
    public ProcessWorker(PortManager portManager, ChannelReader<MatchCreateInfo> reader)
    {
        _portManager = portManager;
        _reader = reader;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        
        try
        {
            await foreach(var matchInfo in _reader.ReadAllAsync(ct))
            {
            
                if (_portManager.TryGetPort(out ushort port))
                {
                    Console.WriteLine($"[Worker] Got port {port} for Match {matchInfo.MatchId}. Starting Godot process...");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[Worker] No free ports avalible for match start!");
                }

            }
        }
        catch  (OperationCanceledException)
        {
            Console.WriteLine("[Worker] Process worker stopped gracefully.");
        }
        

    }

}