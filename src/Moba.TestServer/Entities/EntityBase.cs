using System.Numerics;
using Entities.Interfaces;
using Moba.Shared.UdpServer.Heroes;

namespace Entities;

public class EntityBase : IDamagable
{

    public event Action<EntityBase>? OnDie; // killer
    

    public ushort GlobalId {get;}
    public MatchHeroes UnitId {get;}
    public MacthHeroesTypes UnitType {get;}



    public Vector3 Position {get; protected set;}
    public Vector3 SpawnPosition {get; protected set;}
    public Vector3 Rotation {get; protected set;} 
    public float Radius {get; protected set;}
    protected float _health;
    protected float _maxHealth;
    public float Health
    {
        get => _health;
        set => _health = Math.Clamp(value, 0, _maxHealth);
    }
    public bool IsDead {get; private set;} = false;
    public string Name {get; private set;} = "Unit";

    protected readonly EntityParams _dllData;

    public EntityBase(ushort globalId, MatchHeroes unitId, Vector3 spawnPosition)
    {
        
        if(!HeroesLibrary.TryGet(unitId, out var dllData)) Console.WriteLine($"[EntityBase] Id:{GlobalId} unit constructor broken. Can't find data in dll.");
            
        _dllData = dllData;

        SpawnPosition = spawnPosition;
        Name = dllData.EntityName;
        Radius = dllData.Radius;
        UnitId = unitId;
        GlobalId = globalId;
        UnitType = dllData.UnitType;

        _maxHealth = dllData.BaseHealth;
        _health = dllData.BaseHealth;

        Rotation = new Vector3(0, 0, 0);


    }

    public virtual void TakeDamage(float damage, DamageTypes dmgType, EntityBase damager)
    {
        
        Health -= damage;


        if(_health == 0)
        {
            IsDead = true;
            OnDie?.Invoke(damager);
        }

    }

}