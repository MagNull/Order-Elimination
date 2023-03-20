using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleEntity : IActionTarget
    {
        public bool TakeDamage(DamageInfo damageInfo);
        public bool ApplyEffect(IEffect effect); //нестакающиеся эффекты не должны добавляться
    }
}
