
using Moba.Shared.UdpServer.Heroes;

namespace Entities.Interfaces;

public interface IDamagable
{
    
    void TakeDamage(float damage, DamageTypes dmgType, EntityBase damager);

}