using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IEffectHolder
    {
        public IEnumerable<IEffect> Effects { get; }
        public event Action<IEffect> EffectAdded;
        public event Action<IEffect> EffectRemoved;

        public bool HasEffect(IEffect effect);
        public bool CanApplyEffect(IEffect effect);// => !HasEffect(effect) || effect.EffectData.IsStackable;
        public bool ApplyEffect(IEffect effect, IAbilitySystemActor applier);
        public bool RemoveEffect(IEffect effect);
    }
}
