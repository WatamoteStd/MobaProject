using Godot;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Teams;
using System;

public partial class BaseEntity : CharacterBody3D
{
	
	// EXPORT FIELDS
	[Export] private ProgressBar _healthBar;

	public ushort ServerId {get; private set;}
	public MatchHeroes UnitId {get; private set;}
	public MacthHeroesTypes UnitType {get; private set;}
	public MatchTeams Team {get; private set;}

	// MODEL
	[Export] protected Node3D _modelContainer;
	[Export] protected CollisionShape3D _collisionShape;


	protected Vector3 _spawnPosition;
	protected Vector3 _moveTarget = Vector3.Zero;
	protected float _health;
	protected float _maxHealth;
	protected float _healthRegenearation;
	public float Health
	{
		get => _health;
		set
		{
			_health = Math.Clamp(value, 0, _maxHealth);
			UpdateHealthBar();
		}
	}

	protected float _moveSpeed;
	public float MoveSpeed
	{
		get => _moveSpeed;
		set => _moveSpeed = value;
	}

	protected float _rotationSpeed;


	// FUNCTIONS ========================================================

	/// <summary>
	/// Init entity by defaulth data from Shared DLL and ClientAssetsLibrary
	/// </summary>
	/// <param name="hero"></param>
	public virtual void InitDefault(MatchHeroes hero, ushort serverId, Vector3 spawnPosition, MatchTeams team) // like ready
	{
		
		if(!ClientAssetLibrary.TryGetUnitInfo(hero, out var unitInfo))
		{
			GD.Print($"[BaseEntity] Can't spawn broken ClientAssetsLibrary.");
			return;
		}
		if(!HeroesLibrary.TryGet(hero, out var dllInfo))
		{
			GD.Print($"[BaseEntity] Can't spawn broken DllLibrary.");
			return;
		}

		if(_collisionShape.Shape is CylinderShape3D cylinder)
		{
			cylinder.Radius = dllInfo.Radius;
		}

		UnitId = hero;  
		ServerId = serverId;
		UnitType = dllInfo.UnitType;

		_spawnPosition = spawnPosition;

		_maxHealth = dllInfo.BaseHealth;
		_health = dllInfo.BaseHealth;
		_healthRegenearation = dllInfo.HealthRegen;

		_moveSpeed = dllInfo.BaseSpeed;
		_rotationSpeed = dllInfo.RotationSpeed;

		Team = team;

		InitiateModel();

		RecolorHealthBar();

	}



	protected virtual void UpdateHealthBar()
	{
		_healthBar.MaxValue = _maxHealth;
		_healthBar.Value = _health;
	}

	protected virtual void InitiateModel()
	{   
		if(!ClientAssetLibrary.TryGetUnitInfo(UnitId, out var data)) { GD.Print($"[BaseEntity] IntiateModel : Can't find info in client library."); return; }

		var modelPrefab = GD.Load<PackedScene>(data.ModelPath);

		var model = modelPrefab.Instantiate();
		_modelContainer.AddChild(model);
	}

	private void RecolorHealthBar()
	{
		if(Team != MatchSession.Instance.LocalPlayerTeam)
		{
			_healthBar.Modulate = Colors.Red;
		}
		else _healthBar.Modulate = Colors.Green;
	}


}
