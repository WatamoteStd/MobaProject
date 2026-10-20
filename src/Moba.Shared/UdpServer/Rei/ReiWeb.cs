
using System.Net;
using Moba.Shared.UdpServer.Rei.Interfaces;

namespace Moba.Shared.UdpServer.Rei;

public class ReiWeb 
{

    public event Action OnCloneConnected;
    
    private readonly ISocketOwner _socketOwner;
    
    private readonly Dictionary<EndPoint, ReiClone> _endToRei = new();
    private readonly ReaderWriterLockSlim _lock = new();
    public int TotalCount
    {
        get
        {
            _lock.EnterReadLock();
            try { return _endToRei.Count; }
            finally {  _lock.ExitReadLock();  }
        }
    }

    public ReiWeb(ISocketOwner socketOwner)
    {
        _socketOwner = socketOwner;
    }

    public void ConnectRei(EndPoint endPoint, ReiClone rei)
    {
        _lock.EnterWriteLock();

        try
        {
            _endToRei[endPoint] = rei;
            OnCloneConnected?.Invoke();
        }
        finally
        {
            _lock.ExitWriteLock();
        }

    }

    public void RemoveRei(EndPoint endPoint)
    {

        _lock.EnterWriteLock();
        
        try
        {
            _endToRei.Remove(endPoint);
        }
        finally
        {
            _lock.ExitWriteLock();
        }

    }

    public bool HandleIncomingPacket(ReadOnlySpan<byte> rawData, EndPoint sender)
    {
        
        _lock.EnterReadLock();

        try
        {
            
            if(_endToRei.TryGetValue(sender, out var rei))
            {
                
                return rei.ReceivedBytes(rawData, sender);

            }
            return false;

        }
        finally
        {
            _lock.ExitReadLock();
        }
        
    }

    public void SendTo(ReadOnlySpan<byte> buffer, int length, EndPoint endPoint)
    {
        
        _socketOwner.SendTo(buffer, length, endPoint);

    }



}