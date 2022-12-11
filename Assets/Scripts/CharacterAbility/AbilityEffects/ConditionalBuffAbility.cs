using System;
using CharacterAbility.BuffEffects;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class ConditionalBuffAbility : Ability
    {
        private readonly BuffType _buffType;
        private readonly int _value;
        private readonly BuffConditionType _conditionType;
        private ITickEffect _buff;

        public ConditionalBuffAbility(IBattleObject caster, Ability effects, float probability, BuffType buffType,
            int value, BuffConditionType conditionType, BattleObjectSide filter) :
            base(caster, effects, filter, probability)
        {
            _buffType = buffType;
            _value = value;
            _conditionType = conditionType;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (_filter != BattleObjectSide.None && target.Side != _filter)
                return;

            //TODO: Fix duration trick
            InitBuff();
            target.AddTickEffect(_buff);
            switch (_conditionType)
            {
                case BuffConditionType.Damaged:
                    //target.Damaged += RemoveBuff;
                    break;
                case BuffConditionType.Moved:
                    Action<Cell, Cell> removeBuff = null;
                    removeBuff = (_, _) =>
                    {
                        Debug.Log("Buff removed");
                        target.RemoveTickEffect(_buff);
                        target.Moved -= removeBuff;
                    };

                    target.Moved += removeBuff;
                    break;
            }
        }

        private void InitBuff()
        {
            //TODO: Fix duration trick
            _buff = _buffType switch
            {
                BuffType.Evasion => new EvasionStatsBuff(_value, 9999),
                BuffType.Attack => new EvasionStatsBuff(_value, 9999),
                BuffType.Health => new EvasionStatsBuff(_value, 9999),
                BuffType.Movement => new EvasionStatsBuff(_value, 9999),
                BuffType.IncomingAccuracy => new IncomingBuff(IncomingDebuffType.Accuracy, 9999,
                    _value),
                BuffType.IncomingAttack => new IncomingBuff(IncomingDebuffType.Attack, 9999,
                    _value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}