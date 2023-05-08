using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IEffectHolder
    {
        public IEnumerable<BattleEffect> Effects { get; }
        public event Action<BattleEffect> EffectAdded;
        public event Action<BattleEffect> EffectRemoved;

        public bool HasEffect(IEffectData effect);
        public bool ApplyEffect(IEffectData effect, AbilitySystemActor applier);
        public bool RemoveEffect(BattleEffect effectSelector);
    }
}
