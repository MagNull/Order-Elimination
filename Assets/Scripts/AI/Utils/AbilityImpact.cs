using System;
using System.Linq;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
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
        public int AffectedEnemies;
        public int AffectedAlies;

        private readonly IActiveAbilityData _data;
        private readonly IBattleContext _battleContext;
        private readonly AbilitySystemActor _caster;
        private readonly Vector2Int _targetPos;

        public AbilityImpact(
            IActiveAbilityData data, IBattleContext battleContext, AbilitySystemActor caster, Vector2Int targetPos)
        {
            _data = data;
            _battleContext = battleContext;
            _caster = caster;
            _targetPos = targetPos;
            CurrentDamage = 0;
            CurrentHeal = 0;
            RawDamage = 0;
            RawHeal = 0;
            AffectedEnemies = 0;
            AffectedAlies = 0;
            StartProcess();
        }

        private void StartProcess()
        {
            if (!TryGetTargetGroups(out var targetGroups))
            {
                //TODO: fix error
                //Logging.LogError($"Failed to peek Target Groups for {_data.View.Name}");
            }
            var instructions = _data.Execution.ActionInstructions;
            foreach (var abilityInstruction in instructions)
            {
                ProcessInstruction(abilityInstruction, targetGroups);
            }
            //Debug.Log(_data.View.Name + ": " + AffectedEnemies);
            _data.TargetingSystem.CancelTargeting();
        }

        private bool TryGetTargetGroups(out CellGroupsContainer targetGroups)
        {
            switch (_data.TargetingSystem)
            {
                case IRequireSelectionTargetingSystem manualTargetingSystem:
                    if (!manualTargetingSystem.TryPeekDistribution(out targetGroups, _battleContext, _caster, _targetPos))
                    {
                        //Fails when tries to use Movement on enemy entity (which is obviously wouldn't work)
                        targetGroups = CellGroupsContainer.Empty;
                        return false;
                    }
                    return true;
                case NoTargetTargetingSystem noTargetSystem:
                    targetGroups = noTargetSystem.PeekDistribution(_battleContext, _caster);
                    return true;
                default:
                    Logging.LogError("Not supported targeting system");
                    targetGroups = CellGroupsContainer.Empty;
                    return false;
            }
        }

        private void ProcessInstruction(AbilityInstruction instruction, CellGroupsContainer targetGroups)
        {
            foreach (var cellGroupNumber in instruction.AffectedCellGroups)
            {
                if (!targetGroups.ContainsGroup(cellGroupNumber))
                {
                    CalculateRawDamage(instruction, targetGroups);
                    continue;
                }

                foreach (var cell in targetGroups.GetGroup(cellGroupNumber))
                    CalculateCurrentImpactFromCell(instruction, cell, targetGroups);
            }

            var onSuccess = instruction.InstructionsOnActionSuccess;
            foreach (var abilityInstruction in onSuccess)
                ProcessInstruction(abilityInstruction, targetGroups);

            var following = instruction.FollowingInstructions;
            foreach (var abilityInstruction in following)
                ProcessInstruction(abilityInstruction, targetGroups);
        }

        private void CalculateRawDamage(AbilityInstruction instruction, CellGroupsContainer targetGroups)
        {
            //Can throw an exception if value depends on target.
            var actionContext = new ActionContext(_battleContext, targetGroups, _caster, null);
            var calculationContext = ValueCalculationContext.Full(actionContext);

            //Calculate value based on context
            switch (instruction.Action)
            {
                case InflictDamageAction inflictDamageAction:
                    if (!inflictDamageAction.DamageSize.CanBePrecalculatedWith(calculationContext))
                        Logging.LogException(new NotEnoughDataArgumentException());
                    RawDamage += inflictDamageAction.DamageSize.GetValue(calculationContext) * instruction.RepeatNumber;
                    break;
                case HealAction healAction:
                    if (!healAction.HealSize.CanBePrecalculatedWith(calculationContext))
                        Logging.LogException(new NotEnoughDataArgumentException());
                    RawHeal += healAction.HealSize.GetValue(calculationContext) * instruction.RepeatNumber;
                    break;
            }
        }

        private void CalculateCurrentImpactFromCell(
            AbilityInstruction instruction, Vector2Int cell, CellGroupsContainer cellGroups)
        {
            //Determine target type
            AbilitySystemActor[] targets = instruction.Action.ActionRequires switch
            {
                ActionRequires.Maker => new[] { _caster },
                ActionRequires.Cell => new AbilitySystemActor[] { },
                ActionRequires.Target => _battleContext.BattleMap.GetContainedEntities(cell).ToArray(),
                _ => new AbilitySystemActor[] { }
            };
            //If there no targets on cell - skip
            if (!targets.Any())
                return;

            foreach (var target in targets)
            {
                //Form action context
                var actionContext = new ActionContext(_battleContext, cellGroups, _caster, target);
                var calculationContext = ValueCalculationContext.Full(actionContext);

                //Calculate value based on context
                switch (instruction.Action)
                {
                    case InflictDamageAction inflictDamageAction:
                        CurrentDamage += inflictDamageAction.DamageSize.GetValue(calculationContext) *
                                         instruction.RepeatNumber;
                        RawDamage = CurrentDamage;
                        break;
                    case HealAction healAction:
                        CurrentHeal += healAction.HealSize.GetValue(calculationContext) * instruction.RepeatNumber;
                        RawDamage = CurrentDamage;
                        break;
                }

                if (target != null)
                {
                    if (actionContext.BattleContext.GetRelationship(_caster.BattleSide, target.BattleSide) ==
                        BattleRelationship.Enemy)
                        AffectedEnemies++;
                    else if (actionContext.BattleContext.GetRelationship(_caster.BattleSide, target.BattleSide) ==
                             BattleRelationship.Ally)
                        AffectedAlies++;
                }
            }
        }
    }
}