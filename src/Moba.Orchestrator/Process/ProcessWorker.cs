
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using Core;
using Moba.Shared.OrchestratorData;
using NATS.Net;

namespace Process;

public class ProcessWorker
{
    private readonly PortManager _portManager;
    private readonly ChannelReader<MatchCreateInfo> _reader;
    private readonly NatsClient _nats;
    private readonly string serverExePath = @"C:\MobaProject\src\Moba.TestServer\bin\Release\net10.0\win-x64\publish\Moba.TestServer.exe";
    public int MemoryDebugTime {get;}


    private ConcurrentDictionary<Guid, System.Diagnostics.Process> _matchIdToProcessId = new();
    
    public ProcessWorker(PortManager portManager, ChannelReader<MatchCreateInfo> reader, int debugTime, NatsClient nats)
    {
        _portManager = portManager;
        _reader = reader;
        MemoryDebugTime = debugTime;
        _nats = nats;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        _ = MemoryReportAsync();
        
        try
        {
            await foreach(var matchInfo in _reader.ReadAllAsync(ct))
            {
            
                if (_portManager.TryGetPort(out ushort port))
                {
                    Console.WriteLine($"[Worker] Got port {port} for Match {matchInfo.MatchId}. Starting server process...");
                    StartServer(port, matchInfo.MatchId);

                    Console.WriteLine($"[Worker] Get packet from matchmaker.");
                    for(int i = 0; i < matchInfo.PlayerIds.Length; i++)
                    {
                        Console.WriteLine($"[Match Player[#{i}] Id:{matchInfo.PlayerIds[i]}]");
                    }

                    var pck = matchInfo with
                    {
                        Port = port,
                        ServerIp = "127.0.0.1"
                    };

                    await _nats.PublishAsync("orchestrator.match.ready", pck, cancellationToken: ct);

                }
                else
                {
                    Console.WriteLine($"[Worker] No free ports avalible for match start!");
                }

            }
        }
        catch  (OperationCanceledException)
        {
            Console.WriteLine("[Worker] Process worker stopped gracefully.");
        }
        

    }

    private void StartServer(ushort port, Guid matchId)
    {
        
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = serverExePath;
        info.Arguments = $"--port {port} --match {matchId}";
        info.UseShellExecute = true; // create another cmd window
        info.CreateNoWindow = false; // false for debug window or true for prod mode
        info.WorkingDirectory = Path.GetDirectoryName(serverExePath);

        var process = new System.Diagnostics.Process { StartInfo = info };
        process.EnableRaisingEvents = true;
        process.Start();

        process.PriorityClass = ProcessPriorityClass.High;

        process.Exited += (sender, e) =>
        {
            Console.WriteLine($"[Process] Server {matchId} stopped with code {process.ExitCode}");
            _portManager.ReleasePort(port);
            _matchIdToProcessId.TryRemove(matchId, out _);
            process.Dispose();
        };

        _matchIdToProcessId[matchId] = process;

    }

    public async Task MemoryReportAsync()
    {
        
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(MemoryDebugTime));

        double usedMemory = 0.0f;

        while (await timer.WaitForNextTickAsync())
        {
            
            try
            {
                
                foreach(var proc in _matchIdToProcessId.Values)
                {
                    if (proc.HasExited) continue;

                    proc.Refresh();
                    long bytes = proc.WorkingSet64;
                    double megabytes = bytes / (1024.0 * 1024.0);
                    usedMemory += megabytes;

                }


            }
            catch (Exception)
            {

            }

            Console.WriteLine($"[Memory Report] Total servers:{_matchIdToProcessId.Count}");
            Console.WriteLine($"[Memory Report] Total RAM usage: {usedMemory:F2} MB");

            usedMemory = 0.0f;
            
        }

    }

}