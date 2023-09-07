using OrderElimination.Infrastructure;
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

        private EffectHumanRepresentation(IEffectData effectData)
        {
            //Bad prefix system
            if (effectData.IncomingActionProcessor != null)
            {
                _parameters.AddRange(
                    GetProcessorParameters(effectData.IncomingActionProcessor, ProcessorType.Incoming));
            }
            if (effectData.OutcomingActionProcessor != null)
            {
                _parameters.AddRange(
                    GetProcessorParameters(effectData.OutcomingActionProcessor, ProcessorType.Outcoming));
            }
        }

        public static EffectHumanRepresentation FromEffect(IEffectData effectData)
        {
            var representation = new EffectHumanRepresentation(effectData);
            return representation;
            //collapse Compound processors
            //add * before parameters in Conditional processors
        }

        private HumanValue[] GetProcessorParameters(IActionProcessor processor, ProcessorType processorType)
        {
            var parameters = new List<HumanValue>();
            var processorPrefix = GetProcessorPrefix(processorType);
            if (processor is CompoundActionProcessor compoundProcessor)
            {
                foreach (var subprocessor in compoundProcessor.Processors)
                {
                    parameters.AddRange(GetProcessorParameters(subprocessor, processorType));
                }
            }
            else if (processor is ConditionalActionProcessor conditionalProcessor)
            {
                //add *
                //continue with inner processor
                parameters.AddRange(GetProcessorParameters(conditionalProcessor, processorType).Select(p => p.AddPrefix("*")));
            }
            else if (processor is DamageActionProcessor damageProcessor)
            {
                var prefix = damageProcessor.AllowedDamageTypes != EnumMask<DamageType>.Full ? "*" : "";
                if (damageProcessor.IsChangingDamage)
                {
                    var dmgInfluence = $"{damageProcessor.DamageOperation.AsString()} {damageProcessor.DamageOperand.DisplayedFormula}";
                    parameters.Add(new($"{prefix}{processorPrefix} урон", dmgInfluence, ValueUnits.None));
                }
                if (damageProcessor.IsChangingAccuracy)
                {
                    var accValue = damageProcessor.AccuracyOperand.DisplayedFormula;
                    var units = ValueUnits.None;
                    if (damageProcessor.AccuracyOperand is ConstValueGetter constOperand)
                    {
                        accValue = (constOperand.Value * 100).ToString();
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
                    var healInfluence = $"{healProcessor.HealOperation.AsString()} {healProcessor.HealOperand.DisplayedFormula}";
                    parameters.Add(new($"{processorPrefix} лечение", healInfluence, ValueUnits.None));
                }
            }
            return parameters.ToArray();
        }
    }
}
