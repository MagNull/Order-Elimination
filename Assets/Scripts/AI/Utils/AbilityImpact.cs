using System.Linq;
using OrderElimination.AbilitySystem;

namespace AI.Utils
{
    public struct AbilityImpact
    {
        public float Damage;
        public float Heal;
        
        private readonly IActiveAbilityData _data;
        private readonly IBattleContext _battleContext;
        private readonly AbilitySystemActor _caster;
        private readonly AbilitySystemActor _target;

        public AbilityImpact(IActiveAbilityData data, IBattleContext battleContext, AbilitySystemActor caster,
            AbilitySystemActor target)
        {
            _data = data;
            _battleContext = battleContext;
            _caster = caster;
            _target = target;
            Damage = 0;
            Heal = 0;
            StartProcess();
        }

        private void StartProcess()
        {
            PrepareTargetingToProcess();
            var instructions = _data.Execution.ActionInstructions;
            foreach (var abilityInstruction in instructions)
            {
                ProcessInstruction(abilityInstruction);
            }
            _data.TargetingSystem.CancelTargeting();
        }

        private void PrepareTargetingToProcess()
        {
            if (_data.TargetingSystem is IRequireTargetsTargetingSystem targetingSystem)
            {
                var availableCells = _data.Rules.GetAvailableCellPositions(_battleContext, _caster);
                targetingSystem.SetAvailableCellsForSelection(availableCells);
            }

            _data.TargetingSystem.StartTargeting(_battleContext.BattleMap.CellRangeBorders,
                _caster.Position);
            switch (_data.TargetingSystem)
            {
                case SingleTargetTargetingSystem singleTargeting:
                {
                    singleTargeting.Select(_target.Position);
                    break;
                }
            }
        }

        private void ProcessInstruction(AbilityInstruction instruction)
        {
            var actionContext = new ActionContext(_battleContext, _data.TargetingSystem.ExtractCastTargetGroups(),
                _caster, _target);
            switch (instruction.Action)
            {
                case InflictDamageAction inflictDamageAction:
                    Damage += inflictDamageAction.DamageSize.GetValue(actionContext);
                    break;
                case HealAction healAction:
                    Heal += healAction.HealSize.GetValue(actionContext);
                    break;
            }

            var onSuccess = instruction.InstructionsOnActionSuccess;
            foreach (var abilityInstruction in onSuccess) 
                ProcessInstruction(abilityInstruction);
        }
    }
}