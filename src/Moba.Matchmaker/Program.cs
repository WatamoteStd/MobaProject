
using Moba.Matchmaker;
using Moba.Matchmaker.Core;
using Moba.Matchmaker.Entities;
using Moba.Shared.MatchmakerLibs;

MatchmakingManager manager = new MatchmakingManager();
MatchmakingEngine engine = new MatchmakingEngine(100, 10);
MatchmakingLoop loop = new MatchmakingLoop(manager, engine);

loop.Start();

Console.WriteLine("=== MATCHMAKER TEST CLI ===");
Console.WriteLine("Команды:");
Console.WriteLine("  add <id> <mmr> <mode> - Добавить игрока (mode: solo, trio, full)");
Console.WriteLine("  bot <count> <mode>   - Закинуть пачку ботов с рандомным MMR");
Console.WriteLine("  stat                 - Показать количество игроков в пулах");
Console.WriteLine("  exit                 - Выход");
Console.WriteLine("===========================\n");

long autoPlayerId = 1;

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim().ToLower();

    if (string.IsNullOrEmpty(input)) continue;

    var parts = input.Split(' ');
    var command = parts[0];

    if (command == "exit")
    {
        loop.Stop();
        break;
    }

    switch (command)
    {
        case "add":
            if (parts.Length < 4)
            {
                Console.WriteLine("Ошибка! Формат: add <id> <mmr> <solo|trio|full>");
                break;
            }

            if (long.TryParse(parts[1], out var id) &&
                int.TryParse(parts[2], out var mmr) &&
                TryParseMode(parts[3], out var mode))
            {
                manager.AddPlayer(new PoolPlayer(id, mmr), mode);
                Console.WriteLine($"[+] Игрок {id} (MMR: {mmr}) добавлен в {mode}");
            }
            else
            {
                Console.WriteLine("Неверные параметры.");
            }
            break;

        case "bot":
            if (parts.Length < 3)
            {
                Console.WriteLine("Ошибка! Формат: bot <count> <solo|trio|full>");
                break;
            }

            if (int.TryParse(parts[1], out var count) && TryParseMode(parts[2], out var botMode))
            {
                var rand = new Random();
                for (int i = 0; i < count; i++)
                {
                    var botMmr = rand.Next(800, 1500);
                    manager.AddPlayer(new PoolPlayer(autoPlayerId++, botMmr), botMode);
                }
                Console.WriteLine($"[+] Добавлено {count} ботов в пул {botMode}");
            }
            break;

        case "stat":
            var soloCount = manager.GetPool(MatchProperty.Solo).GetActivePlayers().Count;
            var trioCount = manager.GetPool(MatchProperty.Trio).GetActivePlayers().Count;
            var fullCount = manager.GetPool(MatchProperty.Full).GetActivePlayers().Count;

            Console.WriteLine($"--- Статистика пулов ---");
            Console.WriteLine($"Solo: {soloCount} игрок(ов)");
            Console.WriteLine($"Trio: {trioCount} игрок(ов)");
            Console.WriteLine($"Full: {fullCount} игрок(ов)");
            
            break;

        default:
            Console.WriteLine("Неизвестная команда.");
            break;
    }
}

// Вспомогательный парсер режима
bool TryParseMode(string str, out MatchProperty property)
{
    property = str switch
    {
        "solo" => MatchProperty.Solo,
        "trio" => MatchProperty.Trio,
        "full" => MatchProperty.Full,
        _ => MatchProperty.Solo
    };

    return str is "solo" or "trio" or "full";
}