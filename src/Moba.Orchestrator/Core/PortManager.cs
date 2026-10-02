
using System.Collections.Concurrent;

namespace Core;

public class PortManager
{
    
    private PortInfo[] _ports;

    public PortManager(int count, int firstIndex)
    {
        _ports = new PortInfo[count];
        
        for (int i = 0; i < count; i++)
        {
            
            _ports[i] = new PortInfo((ushort)(firstIndex + i));

        }

    }

    public bool TryGetPort(out ushort port)
    {
        port = 0;
        
        for(int i = 0; i < _ports.Length; i++)
        {
            
            if (!_ports[i].IsBusy)
            {
                port = _ports[i].Port;
                _ports[i].IsBusy = true;
                return true;
            }

        }
        return false;

    }

    public void ReleasePort(ushort port)
    {
        
        for(int i = 0; i < _ports.Length; i++)
        {
            if (_ports[i].Port == port)
            {
                _ports[i].IsBusy = false;
                return;
            }
        }

    }

    public void ShowPortInfo()
    {
        
        Console.WriteLine($"[Port Manager] Total port count: {_ports.Length}");

        for (int i = 0; i < _ports.Length; i++)
        {
            
            Console.WriteLine($"= Port:{_ports[i].Port} | IsBusy:{_ports[i].IsBusy} =");

        }

    }

}