using Godot;
using Moba.Shared.UdpServer.Heroes;
using System;

public partial class HeroSelectButton : TextureButton
{
	
	[Export] public MatchHeroes HeroId {get; private set;}

}
