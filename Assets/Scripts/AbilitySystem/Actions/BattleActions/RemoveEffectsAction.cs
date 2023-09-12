using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class RemoveEffectsAction : BattleAction<RemoveEffectsAction>
    {
        public enum EffectSpecificationOption
        {
            ByCharacter,
            ByEffectData,
            //ByApplier
        }

        [ShowInInspector, OdinSerialize]
        public EffectSpecificationOption SpecifyEffectBy { get; set; }

        [ShowIf("@" + nameof(SpecifyEffectBy) + " == " + nameof(EffectSpecificationOption) + "." + nameof(EffectSpecificationOption.ByCharacter))]
        [ShowInInspector, OdinSerialize]
        public EnumMask<EffectCharacter> EffectCharacter { get; set; } = EnumMask<EffectCharacter>.Empty;

        [ShowIf("@" + nameof(SpecifyEffectBy) + " == " + nameof(EffectSpecificationOption) + "." + nameof(EffectSpecificationOption.ByEffectData))]
        [ShowInInspector, OdinSerialize]
        public HashSet<IEffectData> EffectsData { get; set; } = new();

        [HorizontalGroup("RestrictByApplier")]
        [ShowInInspector, OdinSerialize]
        public bool RestrictByApplier { get; set; }

        [HorizontalGroup("RestrictByApplier")]
        [ShowIf("@" + nameof(RestrictByApplier))]
        [ShowInInspector, OdinSerialize]
        public ActionEntity AppliedBy { get; set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public override IBattleAction Clone()
        {
            var clone = new RemoveEffectsAction();
            clone.SpecifyEffectBy = SpecifyEffectBy;
            clone.EffectCharacter = EffectCharacter.Clone();
            clone.EffectsData = EffectsData.ToHashSet();
            clone.RestrictByApplier = RestrictByApplier;
            clone.AppliedBy = AppliedBy;
            return clone;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var target = useContext.ActionTarget;
            var targetedEffects = SpecifyEffectBy switch
            {
                EffectSpecificationOption.ByCharacter 
                => target.Effects.Where(e => EffectCharacter[e.EffectData.EffectCharacter]),
                EffectSpecificationOption.ByEffectData 
                => target.Effects.Where(e => EffectsData.Contains(e.EffectData)),
                _ 
                => throw new NotImplementedException(),
            };
            if (RestrictByApplier)
            {
                var targetedApplier = AppliedBy switch
                {
                    ActionEntity.Caster => useContext.ActionMaker,
                    ActionEntity.Target => useContext.ActionTarget,
                    _ => throw new NotImplementedException(),
                };
                targetedEffects = targetedEffects.Where(e => e.EffectApplier == targetedApplier);
            }
            foreach (var effect in targetedEffects.ToArray())
                target.RemoveEffect(effect);
            return new SimplePerformResult(this, useContext, true);
        }
    }
}
