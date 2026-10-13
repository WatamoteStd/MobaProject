
using System.Numerics;
using Moba.Shared.UdpServer.Heroes;

namespace Entities;

public class LivingEntity : EntityBase
{

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

}