using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class EffectGameRepresentation
    {
        private List<DamageRepresentation> _damageRepresentations = new();
        private List<HealRepresentation> _healRepresentations = new();
        //private List<ApplyEffectRepresentation> _effectRepresentations = new();
        private List<ModifyStatsRepresentation> _modifyStatsRepresentations = new();
        private List<TemporaryArmorRepresentation> _tempArmorRepresentations = new();

        //IncomingProcessorMeanings: attack/accuracy/heal
        //OutcomingProcessorMeanings
    }
}
