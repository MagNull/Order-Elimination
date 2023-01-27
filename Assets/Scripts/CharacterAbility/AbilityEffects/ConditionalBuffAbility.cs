using System;
using System.Linq;
using CharacterAbility.BuffEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;
using VContainer;

namespace CharacterAbility.AbilityEffects
{
    //TODO: Remove duplicate code TickingBuff
    public class ConditionalBuffAbility : Ability
    {
        private readonly Buff_Type _buffType;
        private readonly float _value;
        private readonly int _duration;
        private readonly ScaleFromWhom _scaleFromWhom;
        private readonly BuffConditionType _conditionType;
        private readonly DamageType _damageType;
        private readonly bool _isMultiplier;
        private readonly bool _isUnique;
        private readonly ITickEffectView _tickEffectView;
        private readonly IObjectResolver _objectResolver;
        private ITickEffect _buff;

        //TODO: Refactor DI in BuffAbilities
        public ConditionalBuffAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability,
            Buff_Type buffType,
            float value, int duration, ScaleFromWhom scaleFromWhom, BuffConditionType conditionType, BattleObjectSide filter,
            DamageType damageType, bool isMultiplier, bool isUnique, ITickEffectView tickEffectView, IObjectResolver objectResolver) :
            base(caster, isMain, nextEffect, filter, probability)
        {
            _buffType = buffType;
            _value = value;
            _duration = duration;
            _scaleFromWhom = scaleFromWhom;
            _conditionType = conditionType;
            _damageType = damageType;
            _isMultiplier = isMultiplier;
            _isUnique = isUnique;
            _tickEffectView = tickEffectView;
            _objectResolver = objectResolver;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            InitBuff();
            if (_buff.IsUnique && target.AllEffects.Any(ef => ef.Equals(_buff)))
            {
                await UseNext(target, stats);
                return;
            }
            
            target.AddTickEffect(_buff);
            _buff.Tick(target);
            switch (_conditionType)
            {
                case BuffConditionType.Damaged:
                    Action<TakeDamageInfo> removeDamaged = null;
                    removeDamaged = _ =>
                    {
                        _buff.RemoveTickEffect(target);
                        target.Damaged -= removeDamaged;
                    };

                    target.Damaged += removeDamaged;
                    break;
                case BuffConditionType.Moved:
                    Action<Cell, Cell> removeMoved = null;
                    removeMoved = (_, _) =>
                    {
                        _buff.RemoveTickEffect(target);
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
                        _buff.RemoveTickEffect(target);
                        battleCharacter.Casted -= removeCasted;
                    };

                    battleCharacter.Casted += removeCasted;
                    break;
            }

            await UseNext(target, stats);
        }

        //TODO: Refactor to Buff Bank
        private void InitBuff()
        {
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new StatsBuffEffect(_isUnique,Buff_Type.Evasion, _value, _scaleFromWhom, _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Accuracy => new StatsBuffEffect(_isUnique,Buff_Type.Accuracy, _value, _scaleFromWhom, _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Attack => new StatsBuffEffect(_isUnique,Buff_Type.Attack, _value, _scaleFromWhom, _duration, _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Health => new StatsBuffEffect(_isUnique,Buff_Type.Health, _value, _scaleFromWhom, _duration, _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Movement => new StatsBuffEffect(_isUnique,Buff_Type.Movement, _value, _scaleFromWhom, _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.AdditionalArmor => new StatsBuffEffect(_isUnique,Buff_Type.AdditionalArmor, _value, _scaleFromWhom,
                    _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.IncomingAccuracy => new IncomingBuff(_isUnique,Buff_Type.IncomingAccuracy, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(_isUnique,Buff_Type.IncomingDamageIncrease, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(_isUnique,Buff_Type.IncomingDamageReduction, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.Concealment => new ConcealmentBuff(_duration, _isUnique,_objectResolver.Resolve<CharactersBank>(),
                    _tickEffectView),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}