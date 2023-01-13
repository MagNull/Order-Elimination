using System;
using CharacterAbility.BuffEffects;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    
    //TODO: Remove duplicate code TickingBuff
    public class ConditionalBuffAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly Buff_Type _buffType;
        private readonly int _value;
        private readonly BuffConditionType _conditionType;
        private readonly DamageType _damageType;
        private ITickEffect _buff;
        private ITickEffectView _effectView;

        
        public ConditionalBuffAbility(IBattleObject caster, Ability nextEffect, float probability, Buff_Type buffType,
            int value, BuffConditionType conditionType, BattleObjectSide filter, DamageType damageType, ITickEffectView effectView) :
            base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _buffType = buffType;
            _value = value;
            _conditionType = conditionType;
            _damageType = damageType;
            _effectView = effectView;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
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
            _nextEffect?.Use(target, stats);
        }

        private void InitBuff()
        {

            ITickEffectView effectView = _effectView;
            //TODO: Fix duration trick
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new EvasionStatsBuff(_value, 9999, effectView),
                Buff_Type.Attack => new AttackStatsBuff(_value, 9999, effectView),
                Buff_Type.Health => new EvasionStatsBuff(_value, 9999, effectView),
                Buff_Type.Accuracy => new AccuracyStatsBuff(_value, 9999, effectView),
                Buff_Type.Movement => new EvasionStatsBuff(_value, 9999, effectView),
                Buff_Type.IncomingAccuracy => new IncomingBuff(Buff_Type.IncomingAccuracy, 9999,
                    _value, effectView, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(Buff_Type.IncomingDamageIncrease, 9999,
                    _value, effectView, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(Buff_Type.IncomingDamageReduction, 9999,
                    _value, effectView, _damageType),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}