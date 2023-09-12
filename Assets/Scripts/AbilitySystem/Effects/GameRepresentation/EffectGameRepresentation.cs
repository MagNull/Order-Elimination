using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class EffectGameRepresentation
    {
        //IncomingProcessorMeanings: attack/accuracy/heal
        //OutcomingProcessorMeanings
    }

    public class ActionProcessorMeaning
    {
        public enum TargetValueType
        {
            Unknown,
            Attack,
            Heal,
            Accuracy,
        }

        public IActionProcessor ActionProcessor { get; }

        public TargetValueType TargetValue { get; set; }

        public EffectCharacter AffectionCharacter { get; set; } //good/bad
        //DescriptionFunction (one parameter function)
    }
}
