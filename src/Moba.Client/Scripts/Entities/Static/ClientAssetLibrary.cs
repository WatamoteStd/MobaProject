using Godot;
using Moba.Shared.UdpServer.Heroes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

public static class ClientAssetLibrary
{
	
	private static readonly FrozenDictionary<MatchHeroes, UnitAssets> _idToUnit = FrozenDictionary<MatchHeroes, UnitAssets>.Empty;

	static ClientAssetLibrary()
	{
		
		var emptyDict = new Dictionary<MatchHeroes, UnitAssets>
		{
			{ MatchHeroes.FirstHero, new UnitAssets { ModelPath = "res://Scenes/Units/Heroes/FirstHero/FirstHero.tscn" } }  
		};

		_idToUnit = emptyDict.ToFrozenDictionary();

	}

	public static bool TryGetUnitInfo(MatchHeroes unit, out UnitAssets unitAssets) => _idToUnit.TryGetValue(unit, out unitAssets);


}
