using Godot;
using System;
using System.Collections.Generic;

public partial class MatchSession : Node
{
    
    public static MatchSession Instance {get; private set;}

    public Dictionary<ushort, LobbyCachedPlayer> LobbyCachedData = new();

    public override void _Ready()
    {
        if(Instance != null)
        {
            QueueFree();
            return;
        }
        Instance = this;



    }

    public void ClearCache()
    {
        LobbyCachedData.Clear();
    }


}
