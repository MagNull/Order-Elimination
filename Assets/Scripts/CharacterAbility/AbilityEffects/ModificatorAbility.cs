using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ModificatorAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly ModificationType _modificationType;
        private readonly int _modificatorValue;

        public ModificatorAbility(IAbilityCaster caster, Ability nextAbility, ModificationType modificationType,
            int modificatorValue) : base(caster)
        {
            _modificatorValue = modificatorValue;
            _nextAbility = nextAbility;
            _modificationType = modificationType;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            BattleStats modifiedStats = new BattleStats(stats);
            switch (_modificationType)
            {
                case ModificationType.Accuracy:
                    modifiedStats.Accuracy += modifiedStats.Accuracy * _modificatorValue / 100;
                    break;
            }

            _nextAbility.Use(target, modifiedStats, battleMap);
        }
    }
}