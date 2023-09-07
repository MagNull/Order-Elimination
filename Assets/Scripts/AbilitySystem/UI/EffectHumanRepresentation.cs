using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem.UI
{
    public class EffectHumanRepresentation
    {
        //Human Value: { value, units }
        private List<HumanValue> _parameters = new();
        public IReadOnlyList<HumanValue> Parameters => _parameters;

        public EffectHumanRepresentation FromEffect(IEffectData effectData)
        {
            var representation = new EffectHumanRepresentation();
            throw new NotImplementedException();
            //collapse Compound processors
            //add * before parameters in Conditional processors
        }

        public HumanValue[] GetProcessorParameters(IActionProcessor processor)
        {
            var parameters = new List<HumanValue>();
            if (processor is CompoundActionProcessor compoundProcessor)
            {
                foreach (var subprocessor in compoundProcessor.Processors)
                {
                    parameters.AddRange(GetProcessorParameters(subprocessor));
                }
            }
            else if (processor is ConditionalActionProcessor conditionalProcessor)
            {
                //add *
                //continue with inner processor
                parameters.AddRange(GetProcessorParameters(conditionalProcessor).Select(p => p.AddPrefix("*")));
            }
            else if (processor is DamageActionProcessor damageProcessor)
            {
                //if ()
                //if dmg filter != full add *
            }
            else if (processor is HealActionProcessor healProcessor)
            {

            }
            throw new NotImplementedException();
        }
    }
}
