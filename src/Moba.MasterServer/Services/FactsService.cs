
using System.Text.Json;
using Moba.Shared.MasterServerDto;

namespace Services;

public class FactService
{
    
    private readonly List<FactDto> _facts = new();
    private readonly Random _random = new();

    public FactService(IWebHostEnvironment env)
    {
        
        var path = Path.Combine(env.WebRootPath, "facts.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            _facts = JsonSerializer.Deserialize<List<FactDto>>(json) ?? new List<FactDto>();
        }

    }

    public FactDto GetRandomFact()
    {
        if(_facts.Count == 0)
        {
            return new FactDto { Title = "DO YOU KNOW IT?", Text = "No facts loaded at the server." };
        }

        int index = _random.Next(_facts.Count);
        return _facts[index];
    }

}