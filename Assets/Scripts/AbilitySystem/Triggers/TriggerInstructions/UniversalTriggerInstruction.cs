using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class UniversalTriggerInstruction : ITriggerInstruction
    {
        [ValidateInput(
            "@!(" + nameof(TriggerSetup) + " is " + nameof(IEntityTriggerSetup) + ")", 
            "*Will track Caster's condition.")]
        [ShowInInspector, OdinSerialize]
        public ITriggerSetup TriggerSetup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public CasterRelativePattern CellDistributionPattern { get; private set; }

        [ShowInInspector, OdinSerialize]
        public AbilityInstruction[] Instructions { get; private set; }

        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster)
        {
            IBattleTrigger trigger;
            if (TriggerSetup is IContextTriggerSetup contextSetup)
            {
                trigger = contextSetup.GetTrigger(battleContext);
            }
            else if (TriggerSetup is IEntityTriggerSetup entitySetup)
            {
                var trackingEntity = caster;
                trigger = entitySetup.GetTrigger(battleContext, trackingEntity);
            }
            else
                throw new NotImplementedException();
            trigger.Triggered += OnTriggered;
            return trigger;

            async void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var borders = battleContext.BattleMap.CellRangeBorders;
                var cellGroups = CellDistributionPattern.GetAffectedCellGroups(borders, caster.Position);
                var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups);
                foreach (var i in Instructions)
                {
                    await i.ExecuteRecursive(executionContext);
                }
            }
        }
    }
}
