using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public interface IEffectHolder
    {
        public IEnumerable<BattleEffect> Effects { get; }
        public IReadOnlyList<IEffectData> EffectImmunities { get; }

        public event Action<BattleEffect> EffectAdded;
        public event Action<BattleEffect> EffectRemoved;
        public event Action<IEffectData> EffectBlockedByImmunity;

        public bool HasEffect(IEffectData effectData);
        public BattleEffect[] GetEffects(IEffectData effectData);
        public bool ApplyEffect(IEffectData effectData, AbilitySystemActor applier, out BattleEffect appliedEffect);
        public bool RemoveEffect(BattleEffect effect);

        public void AddEffectImmunity(IEffectData effect);
        public bool RemoveEffectImmunity(IEffectData effect);
    }
}
