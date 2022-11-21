using System;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ModificatorAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly ModificatorType _modificatorType;
        private readonly int _modificatorValue;

        public ModificatorAbility(IBattleObject caster, Ability nextAbility, float probability, ModificatorType modificatorType,
            int modificatorValue, BattleObjectSide filter) : base(caster, nextAbility, filter, probability)
        {
            _modificatorValue = modificatorValue;
            _nextAbility = nextAbility;
            _modificatorType = modificatorType;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            BattleStats modifiedStats = new BattleStats(stats);
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
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
            }

            _nextAbility.Use(target, modifiedStats);
        }
    }
}