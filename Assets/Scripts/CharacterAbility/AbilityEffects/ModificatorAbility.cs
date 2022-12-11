﻿using System;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ModificatorAbility : Ability
    {
        private readonly ModificatorType _modificatorType;
        private readonly int _modificatorValue;

        public ModificatorAbility(IBattleObject caster, Ability effects, float probability, ModificatorType modificatorType,
            int modificatorValue, BattleObjectSide filter) : base(caster, effects, filter, probability)
        {
            _modificatorValue = modificatorValue;
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
                        modifiedStats.DamageModificator = DamageModificator.DoubleArmor;
                        break;
                    case ModificatorType.DoubleHealthDamage:
                        modifiedStats.DamageModificator = DamageModificator.DoubleHealth;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}