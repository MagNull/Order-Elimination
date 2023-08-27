using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class EffectCountValueGetter : IContextValueGetter
    {
        [ShowInInspector, OdinSerialize]
        public IEffectData Effect { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ActionEntity CountOn { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool IsAppliedByCaster { get; private set; }

        public string DisplayedFormula
        {
            get
            {
                var entity = CountOn switch
                {
                    ActionEntity.Caster => "Caster",
                    ActionEntity.Target => "Target",
                    _ => throw new NotImplementedException(),
                };
                var effectName = Effect == null || Effect.View == null
                    ? "???"
                    : Effect.View.Name;
                return $"{entity}.EffectsCount[{effectName}]";
            }
        }


        public IContextValueGetter Clone()
        {
            var clone = new EffectCountValueGetter();
            clone.Effect = Effect;
            clone.CountOn = CountOn;
            clone.IsAppliedByCaster = IsAppliedByCaster;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var askingEntity = context.ActionMaker;
            var entity = CountOn switch
            {
                ActionEntity.Caster => context.ActionMaker,
                ActionEntity.Target => context.ActionTarget,
                _ => throw new NotImplementedException(),
            };
            var effects = entity.GetEffects(Effect);
            if (IsAppliedByCaster)
                return entity.GetEffects(Effect).Where(e => e.EffectApplier == askingEntity).Count();
            else
                return entity.GetEffects(Effect).Count();
        }
    }
}
