using System;
using Cysharp.Threading.Tasks;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class ModificatorAbility : Ability
    {
        private readonly ModificatorType _modificatorType;
        private readonly int _modificatorValue;

        public ModificatorAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability, ModificatorType modificatorType,
            int modificatorValue, BattleObjectType filter) : base(caster, isMain, nextEffect, filter, probability)
        {
            _modificatorValue = modificatorValue;
            _modificatorType = modificatorType;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            BattleStats modifiedStats = new BattleStats(stats);
            switch (_modificatorType)
            {
                case ModificatorType.Accuracy:
                    modifiedStats.Accuracy += _modificatorValue;
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
            await UseNext(target, modifiedStats);
        }
    }
}