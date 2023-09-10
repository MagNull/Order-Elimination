﻿using OrderElimination.Infrastructure;
using OrderElimination.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem.UI
{
    public class EffectHumanRepresentation//static per IEffectData?
    {
        private enum ProcessorType
        {
            Incoming,
            Outcoming
        }

        //Human Value: { value, units }
        private List<HumanValue> _parameters = new();
        public IReadOnlyList<HumanValue> Parameters => _parameters;

        private string GetProcessorPrefix(ProcessorType processorType)
        {
            return processorType switch
            {
                ProcessorType.Incoming => "Вход.",
                ProcessorType.Outcoming => "Исх.",
                _ => throw new NotImplementedException(),
            };
        }

        private EffectHumanRepresentation(IEffectData effectData, ValueCalculationContext calculationContext)
        {
            var parametersHash = new HashSet<HumanValue>();
            foreach (var parameter in effectData.TriggerInstructions.
                Select(t => t.Value)
                .Append(effectData.InstructionOnActivation)
                .Append(effectData.InstructionOnDeactivation)
                .Append(effectData.TemporaryEffectFunctionaity?.OnTimeOutInstruction)
                .Where(i => i != null)
                .SelectMany(i => GetInstructionParameters(i, calculationContext)))
            {
                if (!parametersHash.Contains(parameter))
                {
                    _parameters.Add(parameter);
                    parametersHash.Add(parameter);
                }
            }
            //Bad prefix system
            if (effectData.IncomingActionProcessor != null)
            {
                _parameters.AddRange(
                    GetProcessorParameters(
                        effectData.IncomingActionProcessor, ProcessorType.Incoming, calculationContext));
            }
            if (effectData.OutcomingActionProcessor != null)
            {
                _parameters.AddRange(
                    GetProcessorParameters(
                        effectData.OutcomingActionProcessor, ProcessorType.Outcoming, calculationContext));
            }
        }

        public static EffectHumanRepresentation FromEffect(
            IEffectData effectData, ValueCalculationContext calculationContext)
        {
            var representation = new EffectHumanRepresentation(effectData, calculationContext);
            return representation;
            //collapse Compound processors
            //add * before parameters in Conditional processors
        }

        private IEnumerable<HumanValue> GetProcessorParameters(
            IActionProcessor processor, 
            ProcessorType processorType, 
            ValueCalculationContext calculationContext)
        {
            var parameters = new List<HumanValue>();
            var processorPrefix = GetProcessorPrefix(processorType);
            if (processor is CompoundActionProcessor compoundProcessor)
            {
                foreach (var subprocessor in compoundProcessor.Processors)
                {
                    parameters.AddRange(
                        GetProcessorParameters(subprocessor, processorType, calculationContext));
                }
            }
            else if (processor is ConditionalActionProcessor conditionalProcessor)
            {
                parameters.AddRange(
                    GetProcessorParameters(conditionalProcessor, processorType, calculationContext)
                    .Select(p => p.AddPrefix("*")));
            }
            else if (processor is DamageActionProcessor damageProcessor)
            {
                var prefix = damageProcessor.AllowedDamageTypes != EnumMask<DamageType>.Full ? "*" : "";
                if (damageProcessor.IsChangingDamage)
                {
                    var damage = damageProcessor.DamageOperand.GetSimplifiedFormula(calculationContext);
                    var dmgInfluence = $"{damageProcessor.DamageOperation.AsString()} {damage}";
                    parameters.Add(new($"{prefix}{processorPrefix} урон", dmgInfluence, ValueUnits.None));
                }
                if (damageProcessor.IsChangingAccuracy)
                {
                    var accValue = damageProcessor.AccuracyOperand.GetSimplifiedFormula(calculationContext);
                    var units = ValueUnits.None;
                    if (damageProcessor.AccuracyOperand.CanBePrecalculatedWith(calculationContext))
                    {
                        accValue = (damageProcessor.AccuracyOperand.GetValue(calculationContext) * 100).ToString();
                        units = ValueUnits.Percents;
                    }
                    var accInfluence = $"{damageProcessor.AccuracyOperation.AsString()} {accValue}";
                    parameters.Add(new($"{prefix}{processorPrefix} точн.", accInfluence, units));
                }
                if (damageProcessor.ArmorMultiplier != 1)
                    parameters.Add(new($"{prefix}{processorPrefix} урон броне", damageProcessor.ArmorMultiplier, ValueUnits.Multiplier));
                if (damageProcessor.HealthMultiplier != 1)
                    parameters.Add(new($"{prefix}{processorPrefix} урон здоровью", damageProcessor.HealthMultiplier, ValueUnits.Multiplier));
                if (damageProcessor.IgnoreEvasion)
                    parameters.Add(new($"{prefix}{processorPrefix} игнор. уклонения"));
                //... damageType, damagePriority changes
            }
            else if (processor is HealActionProcessor healProcessor)
            {
                if (healProcessor.IsChangingHeal)
                {
                    var heal = healProcessor.HealOperand.GetSimplifiedFormula(calculationContext);
                    var healInfluence = $"{healProcessor.HealOperation.AsString()} {heal}";
                    parameters.Add(new($"{processorPrefix} лечение", healInfluence, ValueUnits.None));
                }
            }
            else throw new NotImplementedException();
            return parameters;
        }

        private IEnumerable<HumanValue> GetInstructionParameters(
            IEffectInstruction instruction, 
            ValueCalculationContext calculationContext)
        {
            if (instruction == null)
                throw new ArgumentNullException();
            var parameters = new List<HumanValue>();
            if (instruction is CompoundEffectInstruction compoundInstruction)
            {
                foreach (var subInstruction in compoundInstruction.Instructions)
                {
                    parameters.AddRange(GetInstructionParameters(subInstruction, calculationContext));
                }
            }
            else if (instruction is ConditionalEffectInstruction conditionalInstruction)
            {
                parameters.AddRange(
                    GetInstructionParameters(conditionalInstruction.Instruction, calculationContext)
                    .Select(p => p.AddPrefix("*")));
            }
            else if (instruction is EffectActionInstruction actionInstruction)
            {
                var action = actionInstruction.BattleAction;
                if (action is InflictDamageAction damageAction)
                {
                    parameters.Add(new("Урон", damageAction.DamageSize.GetSimplifiedFormula(calculationContext)));
                }
                else if (action is HealAction healAction)
                {
                    var name = healAction.HealPriority switch
                    {
                        LifeStatPriority.ArmorFirst => "Лечение",
                        LifeStatPriority.HealthFirst => "Лечение",
                        LifeStatPriority.ArmorOnly => "Броня",
                        LifeStatPriority.HealthOnly => "Здоровье",
                        _ => throw new NotImplementedException(),
                    };
                    parameters.Add(new("Урон", healAction.HealSize.GetSimplifiedFormula(calculationContext)));
                }
                else if (action is ModifyStatsAction modifyStatsAction)
                {
                    var stat = modifyStatsAction.TargetBattleStat;
                    var statName = Localization.Localization.Current.GetBattleStatFullName(stat);

                    //!!! This works wrong as it can calculate based on already modified stat !!!
                    //if (modifyStatsAction.ValueModifier.CanBePrecalculatedWith(calculationContext))
                    //{

                    //}
                    //var statValue = modifyStatsAction.ValueModifier.CanBePrecalculatedWith(calculationContext)
                    //    ? modifyStatsAction.ValueModifier.GetValue(calculationContext).ToString()
                    //    : modifyStatsAction.ValueModifier.GetSimplifiedFormula(calculationContext);
                    //statValue = stat == BattleStat.Accuracy
                    //    ? "изменена"
                    //    : stat == BattleStat.AttackDamage ? "изменен" : "изменено";
                    parameters.Add(new(statName, "*"));
                }
                //next instructions
            }
            return parameters;
        }
    }
}
