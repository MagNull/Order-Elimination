using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class AbilityGameRepresentation : IAbilityGameRepresentation
    {
        public AbilityType AbilityType { get; private set; }
        public int CooldownTime { get; private set; }

        public TargetingSystemRepresentation TargetingSystem { get; private set; }
        public float? MaxRange { get; private set; }
        public IReadOnlyList<DamageRepresentation> DamageRepresentations { get; private set; }

        //AbilityTags[] Tags; //Melee, Range, Damage, ...
        //ActivationType: Manual, Automatic, Combined
        //Cell groups mapping (0 - MainTarget, 1, 3 - Area, ...)

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

            void DescribeInstruction(AbilityInstruction instruction, int parentTotalRepetitions)
            {
                var localRepetitions = instruction.RepeatNumber;
                var totalRepetitions = localRepetitions * parentTotalRepetitions;

                if (instruction.Action is InflictDamageAction damageAction)
                {
                    damageRepresentations.Add(new(damageAction, localRepetitions, totalRepetitions));
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
