
using System.Collections.Frozen;

namespace Moba.Shared.UdpServer.Heroes;

public static class HeroesLibrary
{
    
    private readonly static FrozenDictionary<MatchHeroes, EntityParams> _entities = FrozenDictionary<MatchHeroes, EntityParams>.Empty;

    public const int MAX_ATTACK_SPEED = 1200;
    public const float MAX_MOVE_SPEED = 120;
    public const float BASE_ATTACK_SPEED = 100;

    static HeroesLibrary()
    {
        
        var tempDic = new Dictionary<MatchHeroes, EntityParams>
        {
            { MatchHeroes.FirstHero, new EntityParams
            {
                BaseHealth = 320, AbilitySlots = 4, Armor = 2, AttackRange = 5f, AttackType = UnitAttackTypes.Melee,
                BaseAttackInterval = 1.8f, BaseDamage = 31, BaseMana = 145, BaseSpeed = 3f, HealthRegen = 0.5f,
                Level = 0, MagicArmor = 0, ManaRegen = 0.35f, RotationSpeed = 1.0f, UnitType = MacthHeroesTypes.Hero,
                Radius = 1.8f, EntityName = "First Hero"
            }
            }
        };

        _entities = tempDic.ToFrozenDictionary();

    }

    public static bool TryGet(MatchHeroes unit, out EntityParams entityParams) => _entities.TryGetValue(unit, out entityParams);

}