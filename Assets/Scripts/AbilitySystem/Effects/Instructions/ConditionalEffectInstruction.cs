using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class ConditionalEffectInstruction : IEffectInstruction
    {
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] CommonConditions { get; private set; } = new ICommonCondition[0];

        [ShowInInspector, OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; } = new IEntityCondition[0];

        [ShowInInspector, OdinSerialize]
        public EffectEntity AskingEntity { get; private set; }

        [EnableIf("@" + nameof(EntityConditions) + " != null && " + nameof(EntityConditions) + ".Length > 0")]
        [ShowInInspector, OdinSerialize]
        public EffectEntity EntityToCheck { get; private set; }

        [PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
        [ShowInInspector, OdinSerialize]
        public IEffectInstruction Instruction { get; private set; }

        public async UniTask Execute(BattleEffect effect)
        {
            var battleContext = effect.BattleContext;
            var askingEntity = AskingEntity switch
            {
                EffectEntity.EffectHolder => effect.EffectHolder,
                EffectEntity.EffectApplier => effect.EffectApplier,
                _ => throw new System.NotImplementedException(),
            };
            var entityToCheck = EntityToCheck switch
            {
                EffectEntity.EffectHolder => effect.EffectHolder,
                EffectEntity.EffectApplier => effect.EffectApplier,
                _ => throw new System.NotImplementedException(),
            };
            if (CommonConditions.All(c => c.IsConditionMet(battleContext, askingEntity))
                && EntityConditions.All(c => c.IsConditionMet(battleContext, askingEntity, entityToCheck)))
            {
                await Instruction.Execute(effect);
            }

        }
    }
}
