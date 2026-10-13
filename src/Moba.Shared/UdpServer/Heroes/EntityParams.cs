
namespace Moba.Shared.UdpServer.Heroes;

public readonly struct EntityParams
{
    
    public required float BaseHealth {get; init;}
    public required float HealthRegen {get; init;}
    public required float BaseMana {get; init;}
    public required float ManaRegen {get; init;}
    public required float BaseSpeed {get; init;}
    public required float Armor {get; init;}
    public required float MagicArmor {get; init;}
    public required float BaseAttackInterval {get; init;}
    public required int Level {get; init;}
    public required MacthHeroesTypes UnitType {get; init;}
    public required float BaseDamage {get; init;}
    public required int AbilitySlots {get; init;}
    public required float RotationSpeed {get; init;}
    public required float AttackRange {get; init;}
    public required UnitAttackTypes AttackType {get; init;}
    public required float Radius {get; init;}
    public required string EntityName {get; init;}

}