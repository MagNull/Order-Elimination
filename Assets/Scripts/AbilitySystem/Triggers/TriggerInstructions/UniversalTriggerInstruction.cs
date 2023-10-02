using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class UniversalTriggerInstruction : ITriggerInstruction
    {
        [VerticalGroup("TriggerSection", PaddingBottom = 10)]
        [BoxGroup("TriggerSection/Trigger", ShowLabel = false)]
        [ValidateInput(
            "@!(" + nameof(TriggerSetup) + " is " + nameof(IEntityTriggerSetup) + ")", 
            "*Will track Caster's condition.")]
        [ShowInInspector, OdinSerialize]
        public ITriggerSetup TriggerSetup { get; private set; }

        [VerticalGroup("RulesSection", PaddingBottom = 10)]
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] CommonConditions { get; private set; }

        [VerticalGroup("RulesSection")]
        [BoxGroup("RulesSection/Distributor", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public ICellGroupsDistributor GroupDistributor { get; private set; }

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
            {
                Logging.LogException( new NotImplementedException());
                throw new NotImplementedException();
            }
            trigger.Triggered += OnTriggered;
            return trigger;

            async void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var cellGroups = GroupDistributor.DistributeSelection(
                    battleContext, caster, new Vector2Int[0]);
                if (CommonConditions != null
                    && !CommonConditions.AllMet(battleContext, caster, cellGroups))
                    return;
                var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups);
                foreach (var i in Instructions)
                {
                    await i.ExecuteRecursive(executionContext);
                }
            }
        }
    }
}
