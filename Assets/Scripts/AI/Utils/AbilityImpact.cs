using System;
using System.Linq;
using OrderElimination.AbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Utils
{
    public struct AbilityImpact
    {
        public float CurrentDamage;
        public float CurrentHeal;
        public float RawDamage;
        public float RawHeal;

        private readonly IActiveAbilityData _data;
        private readonly IBattleContext _battleContext;
        private readonly AbilitySystemActor _caster;
        private readonly Vector2Int _targetPos;

        public AbilityImpact(IActiveAbilityData data, IBattleContext battleContext, AbilitySystemActor caster,
            Vector2Int targetPos)
        {
            _data = data;
            _battleContext = battleContext;
            _caster = caster;
            _targetPos = targetPos;
            CurrentDamage = 0;
            CurrentHeal = 0;
            RawDamage = 0;
            RawHeal = 0;
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
                    singleTargeting.Select(_targetPos);
                    break;
                }
            }
        }

        private void ProcessInstruction(AbilityInstruction instruction)
        {
            var executionContext = new AbilityExecutionContext(_battleContext, _caster,
                _data.TargetingSystem.ExtractCastTargetGroups());
            foreach (var cellGroupNumber in instruction.AffectedCellGroups)
            {
                if (!executionContext.TargetedCellGroups.ContainsGroup(cellGroupNumber))
                {
                    CalculateRawDamage(instruction);
                    continue;
                }
                foreach (var cell in executionContext.TargetedCellGroups.GetGroup(cellGroupNumber))
                    CalculateCurrentImpactFromCell(instruction, cell);
            }

            var onSuccess = instruction.InstructionsOnActionSuccess;
            foreach (var abilityInstruction in onSuccess)
                ProcessInstruction(abilityInstruction);

            var following = instruction.FollowingInstructions;
            foreach (var abilityInstruction in following)
                ProcessInstruction(abilityInstruction);
        }

        private void CalculateRawDamage(AbilityInstruction instruction)
        {
            var actionContext = new ActionContext(_battleContext,
                _data.TargetingSystem.ExtractCastTargetGroups(),
                _caster, null, Vector2Int.zero);

            //Calculate value based on context
            switch (instruction.Action)
            {
                case InflictDamageAction inflictDamageAction:
                    RawDamage += inflictDamageAction.DamageSize.GetValue(actionContext) * instruction.RepeatNumber;
                    break;
                case HealAction healAction:
                    RawHeal += healAction.HealSize.GetValue(actionContext) * instruction.RepeatNumber;
                    break;
            }
        }

        private void CalculateCurrentImpactFromCell(AbilityInstruction instruction, Vector2Int cell)
        {
            //Determine target type
            AbilitySystemActor[] targets = instruction.Action.ActionRequires switch
            {
                ActionRequires.Caster => new[] { _caster },
                ActionRequires.Cell => new AbilitySystemActor[] { },
                ActionRequires.Entity => _battleContext.BattleMap.GetContainedEntities(cell).ToArray(),
                _ => new AbilitySystemActor[] { }
            };
            //If there no targets on cell - skip
            if (!targets.Any())
                return;

            foreach (var target in targets)
            {
                //Form action context
                var actionContext = new ActionContext(_battleContext,
                    _data.TargetingSystem.ExtractCastTargetGroups(),
                    _caster, target, cell);

                //Calculate value based on context
                switch (instruction.Action)
                {
                    case InflictDamageAction inflictDamageAction:
                        CurrentDamage += inflictDamageAction.DamageSize.GetValue(actionContext) * instruction.RepeatNumber;
                        RawDamage = CurrentDamage;
                        break;
                    case HealAction healAction:
                        CurrentHeal += healAction.HealSize.GetValue(actionContext) * instruction.RepeatNumber;
                        RawDamage = CurrentDamage;
                        break;
                }
            }
        }
    }
}