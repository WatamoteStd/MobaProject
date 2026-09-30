namespace Moba.Matchmaker.Entities;

public class PoolPlayer
{
    
    public long PlayerId {get; set;}
    public DateTime JoinedQueue {get; set;}
    public int MMR {get; set;}

    public PoolPlayer(long id, int mmr)
    {
        PlayerId = id;
        JoinedQueue = DateTime.UtcNow;
        MMR = mmr;
    }

}