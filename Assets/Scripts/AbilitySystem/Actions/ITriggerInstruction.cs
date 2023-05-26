using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerInstruction
    {
        public void Activate(IBattleContext battleContext, AbilitySystemActor caster);
    }

    public class EntitiesInZoneTriggerInstruction : ITriggerInstruction
    {
        private bool _isActived;

        [ShowInInspector, OdinSerialize]
        public EntityRelativeZoneTrigger TriggerSetup { get; private set; }

        [ValidateInput("@false", "Use " + nameof(AbilityInstruction.AffectPreviousTarget) + " checkbox.")]
        [ShowInInspector, OdinSerialize]
        public AbilityInstruction InstructionForEnteredEntities { get; private set; }

        [ValidateInput("@false", "Use " + nameof(AbilityInstruction.AffectPreviousTarget) + " checkbox.")]
        [ShowInInspector, OdinSerialize]
        public AbilityInstruction InstructionForLeavedEntities { get; private set; }

        public void Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (_isActived) 
                throw new InvalidOperationException("Trigger instruction has already been activated.");
            _isActived = true;
            var instance = TriggerSetup.GetTrigger(battleContext, caster);
            instance.Triggered += OnTriggered;

            void OnTriggered(ITriggerFireInfo fireInfo)
            {
                Execute((TriggerZoneFireInfo)fireInfo, battleContext, caster);
            }
        }

        private void Execute(TriggerZoneFireInfo info, IBattleContext battleContext, AbilitySystemActor caster)
        {
            var cellGroups = CellGroupsContainer.Empty;
            foreach (var leaver in info.DisappearedEntities)
            {
                //var actionContext = new ActionContext(battleContext, cellGroups, caster, leaver);
                var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups, leaver);
                InstructionForLeavedEntities.ExecuteRecursive(executionContext);
            }
            foreach (var newcomer in info.NewEntities)
            {
                //var actionContext = new ActionContext(battleContext, cellGroups, caster, newcomer);
                var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups, newcomer);
                InstructionForEnteredEntities.ExecuteRecursive(executionContext);
            }
        }
    }
}
