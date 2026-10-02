
namespace Core;

public struct PortInfo
{
    
    public ushort Port {get;}
    public bool IsBusy {get; set;}

    public PortInfo(ushort port)
    {
        Port = port;
        IsBusy = false;

    }

}