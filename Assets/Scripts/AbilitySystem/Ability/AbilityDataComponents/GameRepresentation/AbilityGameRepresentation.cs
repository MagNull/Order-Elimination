using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class AbilityGameRepresentation : IAbilityGameRepresentation
    {
        public AbilityType AbilityType { get; private set; }
        public int CooldownTime { get; private set; }

        public TargetingSystemRepresentation TargetingSystem { get; private set; }
        public float? MaxRange { get; private set; }
        public int? Duration { get; private set; }
        public IReadOnlyList<DamageRepresentation> DamageRepresentations { get; private set; }

        //AbilityTags[] Tags; //Melee, Range, Damage, ...
        //ActivationType: Manual, Automatic, Combined
        //Cell groups mapping (0 - MainTarget, 1, 3 - Area, ...)

        // Hard to calculate damage:
        // - each hit damage can depend on current changing context (20% tarHealth damage per hit)
        // - damage can depend on previous instruction fail/success
        // - values can not always be pre-calculated
        //GetTotalEstimatedDamage()//

        private AbilityGameRepresentation()
        {

        }

        public static AbilityGameRepresentation FromActiveAbility(
            AbilityRules rules,
            int cooldown,
            IAbilityTargetingSystem targetingSystem,
            ActiveAbilityExecution activeFunctional)
        {
            var targetingRepresentation = new TargetingSystemRepresentation(targetingSystem);
            var damageRepresentations = new List<DamageRepresentation>();
            foreach (var instruction in activeFunctional.ActionInstructions)
            {
                DescribeInstruction(instruction, 1);
            }

            var representation = new AbilityGameRepresentation
            {
                AbilityType = AbilityType.Active,
                CooldownTime = cooldown,
                DamageRepresentations = damageRepresentations
            };
            //...
            return representation;

            void DescribeInstruction(
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

                if (instruction.Action is InflictDamageAction damageAction)
                {
                    damageRepresentations.Add(
                        new(affectedEntities, damageAction, localRepetitions, totalRepetitions));
                }

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
            }
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
