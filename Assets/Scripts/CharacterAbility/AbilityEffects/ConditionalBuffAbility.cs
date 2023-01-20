using System;
using CharacterAbility.BuffEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    //TODO: Remove duplicate code TickingBuff
    public class ConditionalBuffAbility : Ability
    {
        private readonly Buff_Type _buffType;
        private readonly float _value;
        private readonly ScaleFromWhom _scaleFromWhom;
        private readonly BuffConditionType _conditionType;
        private readonly DamageType _damageType;
        private readonly bool _isMultiplier;
        private readonly ITickEffectView _tickEffectView;
        private ITickEffect _buff;

        public ConditionalBuffAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability, Buff_Type buffType,
            float value, ScaleFromWhom scaleFromWhom, BuffConditionType conditionType, BattleObjectSide filter,
            DamageType damageType, bool isMultiplier, ITickEffectView tickEffectView) :
            base(caster, isMain, nextEffect, filter, probability)
        {
            _buffType = buffType;
            _value = value;
            _scaleFromWhom = scaleFromWhom;
            _conditionType = conditionType;
            _damageType = damageType;
            _isMultiplier = isMultiplier;
            _tickEffectView = tickEffectView;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            InitBuff();
            target.AddTickEffect(_buff);
            switch (_conditionType)
            {
                case BuffConditionType.Damaged:
                    Action<TakeDamageInfo> removeDamaged = null;
                    removeDamaged = _ =>
                    {
                        Debug.Log("Buff removed");
                        target.RemoveTickEffect(_buff);
                        target.Damaged -= removeDamaged;
                    };

                    target.Damaged += removeDamaged;
                    break;
                case BuffConditionType.Moved:
                    Action<Cell, Cell> removeMoved = null;
                    removeMoved = (_, _) =>
                    {
                        Debug.Log("Buff removed");
                        target.RemoveTickEffect(_buff);
                        target.Moved -= removeMoved;
                    };

                    target.Moved += removeMoved;
                    break;
                case BuffConditionType.Casted:
                    if (target is not BattleCharacter battleCharacter)
                        return;
                    Action removeCasted = null;
                    removeCasted = () =>
                    {
                        Debug.Log("Buff removed");
                        battleCharacter.RemoveTickEffect(_buff);
                        battleCharacter.Casted -= removeCasted;
                    };

                    battleCharacter.Casted += removeCasted;
                    break;
            }

            await UseNext(target, stats);
        }

        private void InitBuff()
        {
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new StatsBuffEffect(Buff_Type.Evasion, _value, _scaleFromWhom, 9999,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Accuracy => new StatsBuffEffect(Buff_Type.Accuracy, _value, _scaleFromWhom, 9999,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Attack => new StatsBuffEffect(Buff_Type.Attack, _value, _scaleFromWhom, 9999, _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Health => new StatsBuffEffect(Buff_Type.Health, _value, _scaleFromWhom, 9999, _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Movement => new StatsBuffEffect(Buff_Type.Movement, _value, _scaleFromWhom, 9999,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.AdditionalArmor => new StatsBuffEffect(Buff_Type.AdditionalArmor, _value, _scaleFromWhom,
                    9999,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.IncomingAccuracy => new IncomingBuff(Buff_Type.IncomingAccuracy, 9999,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(Buff_Type.IncomingDamageIncrease, 9999,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(Buff_Type.IncomingDamageReduction, 9999,
                    _value, _tickEffectView, _damageType),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}