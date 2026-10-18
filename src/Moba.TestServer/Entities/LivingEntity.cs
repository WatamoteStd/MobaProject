
using System.Numerics;
using Moba.Shared.UdpServer.Heroes;

namespace Entities;

public class LivingEntity : EntityBase
{

    public enum States { Idle, Move, Chase, Cast, Attack, Respawn}
    public States CurrentState = States.Idle;


    protected Vector3 _moveTarget = Vector3.Zero;
    protected float _moveSpeed;
    protected float _maxMoveSpeed;
    public float Speed
    {
        get => _moveSpeed;
        set => _moveSpeed = Math.Clamp(value, 0, _maxMoveSpeed);
    }

    protected float _healthRegeneration;
    protected float _mana;
    protected float _maxMana;
    protected float _manaRegeneration;
    public float Mana
    {
        get => _mana;
        set => _mana = Math.Clamp(value, 0, _maxMana);
    }

    // BATTLE STATS
    
    protected float _damage;
    protected float _baseAttackInterval;
    protected float _attackSpeed;
    protected float _maxAttackSpeed;
    public float AttackSpeed
    {
        get => _attackSpeed;
        set => _attackSpeed = Math.Clamp(value, 0, _maxAttackSpeed);
    }

    protected float _attackRange;
    public float AttackRange
    {
        get => _attackRange;
        set => _attackRange = value;
    }

    public UnitAttackTypes AttackType {get; private set;} = UnitAttackTypes.Melee;
    protected float _armor;
    protected float _magicArmor; 
    
    public LivingEntity(ushort globalId, MatchHeroes unitId, Vector3 spawnPosition) : 
        base(globalId, unitId, spawnPosition)
    {
        
        _maxMoveSpeed = HeroesLibrary.MAX_MOVE_SPEED;
        _moveSpeed = _dllData.BaseSpeed;

        _healthRegeneration = _dllData.HealthRegen;
        _maxMana = _dllData.BaseMana;
        _mana = _dllData.BaseMana;
        _manaRegeneration = _dllData.ManaRegen;

        _damage = _dllData.BaseDamage;
        _baseAttackInterval = _dllData.BaseAttackInterval;
        _attackSpeed = HeroesLibrary.BASE_ATTACK_SPEED;
        _maxAttackSpeed = HeroesLibrary.MAX_ATTACK_SPEED;

        _attackRange = _dllData.AttackRange;

    }

    public virtual void Update(float deltaTime)
    {
        
        switch(CurrentState)
        {
            
            case States.Idle:

            break;

            case States.Move:
                {
                    
                    Move(deltaTime);

                }
            break;

        }

    }


    protected virtual void Move(float deltaTime)
    {
        
        var direction = _moveTarget - Position;
        float  distanceSquared = direction.LengthSquared();

        float frameStep = _moveSpeed * deltaTime;

        if(distanceSquared <= frameStep * frameStep)
        {
            Position = _moveTarget;
            CurrentState = States.Idle;
            return;
        }

        var dirNormal = Vector3.Normalize(direction);
        Position += dirNormal * frameStep;

    }




}