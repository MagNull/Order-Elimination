using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class AbilityGameRepresentation : IAbilityGameRepresentation
    {
        private List<DamageRepresentation> _damageRepresentations = new();
        private List<HealRepresentation> _healRepresentations = new();
        private List<AbilityEffectRepresentation> _effectRepresentations = new();

        public AbilityType AbilityType { get; private set; }
        public int CooldownTime { get; private set; }

        public TargetingSystemRepresentation TargetingSystem { get; private set; }
        //public float? MaxRange { get; private set; }
        public int? Duration { get; private set; }
        public IReadOnlyList<DamageRepresentation> DamageRepresentations
            => _damageRepresentations;
        public IReadOnlyList<HealRepresentation> HealRepresentations
            => _healRepresentations;
        public IReadOnlyList<AbilityEffectRepresentation> EffectRepresentations
            => _effectRepresentations;

        //AbilityTags[] Tags; //Melee, Range, Damage, ...
        //ActivationType: Manual, Automatic, Combined
        //Cell groups mapping (0 - MainTarget, 1, 3 - Area, ...)
        //Descriptions for cell groups? (0 - target:enemies, distance:2; ...)

        // Hard to calculate damage:
        // - each hit damage can depend on current changing context (20% tarHealth damage per hit)
        // - damage can depend on previous instruction fail/success
        // - values can not always be pre-calculated
        //GetTotalEstimatedDamage()//

        
        public static AbilityGameRepresentation FromActiveAbility(
            AbilityRules rules,
            int cooldown,
            IAbilityTargetingSystem targetingSystem,
            ActiveAbilityExecution activeFunctional)
        {
            var representation = new AbilityGameRepresentation()
            {
                AbilityType = AbilityType.Active,
                CooldownTime = cooldown,
                TargetingSystem = new TargetingSystemRepresentation(targetingSystem),
            };
            foreach (var instruction in activeFunctional.ActionInstructions)
            {
                representation.DescribeInstruction(instruction, 1);
            }
            //...
            return representation;
        }

        private void DescribeInstruction(
                AbilityInstruction instruction,
                int parentTotalRepetitions)
        {
            var localRepetitions = instruction.RepeatNumber;
            var totalRepetitions = localRepetitions * parentTotalRepetitions;
            //Target identification
            var affectedEntities = EntityFilter.AllowAllFilter;
            var filterConditions = instruction.TargetConditions
                .Select(c => c as EntityFilterCondition)
                .Where(c => c != null)
                .ToArray();
            if (filterConditions.Length > 1) //TODO: implement intersection (*) operation
                throw new System.NotSupportedException("Multiple filters should be multiplied");
            if (filterConditions.Length > 0)
                affectedEntities = filterConditions.First().EntityFilter;
            if (instruction.AffectPreviousTarget)
            {
                //take parent instruction filter recursively
                //intesect with parent filters
            }
            //Target identification
            #region BattleActions
            if (instruction.Action is InflictDamageAction damageAction)
            {
                _damageRepresentations.Add(
                    new(affectedEntities, damageAction, localRepetitions, totalRepetitions));
            }
            else if (instruction.Action is HealAction healAction)
            {
                _healRepresentations.Add(
                    new(affectedEntities, healAction, localRepetitions, totalRepetitions));
            }
            else if (instruction.Action is ApplyEffectAction effectAction)
            {
                _effectRepresentations.Add(new(effectAction.Effect, effectAction.ApplyChance));
            }
            #endregion

            #region Next Instructions
            foreach (var sucInstruction in instruction.InstructionsOnActionSuccess)
            {
                DescribeInstruction(sucInstruction, totalRepetitions);
            }
            foreach (var failInstruction in instruction.InstructionsOnActionFail)
            {
                DescribeInstruction(failInstruction, totalRepetitions);
            }
            foreach (var followInstruction in instruction.FollowingInstructions)
            {
                DescribeInstruction(followInstruction, totalRepetitions);
            }
            #endregion
        }

        public static AbilityGameRepresentation FromPassiveAbility(
        int cooldown,
        PassiveAbilityExecution passiveFunctional)
        {
            //Describe functionality
            var representation = new AbilityGameRepresentation
            {
                AbilityType = AbilityType.Passive,
                CooldownTime = cooldown
                //...
            };
            return representation;
        }
    }
}
