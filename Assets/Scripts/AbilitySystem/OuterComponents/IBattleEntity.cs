using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //К IBattleEntity относятся здания и персонажи
    public interface IBattleEntity
    {
        public bool TakeDamage(DamageInfo incomingDamage, out DamageInfo takenDamage);
        public bool ApplyEffect(IEffect effect); //нестакающиеся эффекты не должны добавляться
    }

    
}
