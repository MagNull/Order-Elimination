using System;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ModificatorAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly ModificatorType _modificatorType;
        private readonly int _modificatorValue;

        public ModificatorAbility(IAbilityCaster caster, Ability nextAbility, ModificatorType modificatorType,
            int modificatorValue) : base(caster)
        {
            _modificatorValue = modificatorValue;
            _nextAbility = nextAbility;
            _modificatorType = modificatorType;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            BattleStats modifiedStats = new BattleStats(stats);
            switch (_modificatorType)
            {
                case ModificatorType.Accuracy:
                    modifiedStats.Accuracy += modifiedStats.Accuracy * _modificatorValue / 100;
                    break;
                case ModificatorType.DoubleArmorDamage:
                    modifiedStats.AttackType = AttackType.DoubleArmor;
                    break;
                case ModificatorType.DoubleHealthDamage:
                    modifiedStats.AttackType = AttackType.DoubleHealth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _nextAbility.Use(target, modifiedStats, battleMap);
        }
    }
}