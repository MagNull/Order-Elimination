using System;
using CharacterAbility.BuffEffects;
using OrderElimination;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAbility
{
    [Serializable]
    public class IncomingBuff : TickEffectBase
    {
        [SerializeField]
        protected int _modificator;
        [SerializeField]
        private readonly Buff_Type _incomingBuffType;
        private readonly DamageType _damageType;

        public IncomingBuff(Buff_Type incomingBuffType, int duration, int modificator,
            DamageType damageType = DamageType.None) : base(duration)
        {
            _incomingBuffType = incomingBuffType;
            _modificator = modificator;
            _damageType = damageType;
        }

        public DamageInfo GetModifiedInfo(DamageInfo info)
        {
            switch (_incomingBuffType)
            {
                case Buff_Type.IncomingAccuracy:
                    if (info.DamageType == _damageType)
                        info.Accuracy += _modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    if (info.DamageType == _damageType)
                        info.Damage *= _modificator;
                    break;
                case Buff_Type.IncomingDamageReduction:
                    if (info.DamageType == _damageType)
                        info.Damage /= _modificator;
                    break;

                case Buff_Type.Attack:
                case Buff_Type.Health:
                case Buff_Type.Accuracy:
                case Buff_Type.Movement:
                case Buff_Type.Evasion:
                default:
                    throw new ArgumentException("Not incoming buff type");
            }

            return info;
        }

        public int GetModifiedValue(int value, Buff_Type buffType)
        {
            var newValue = value;
            Debug.Log(buffType + " " + _incomingBuffType);
            if (_incomingBuffType != buffType)
                return newValue;
            switch (_incomingBuffType)
            {
                case Buff_Type.IncomingAccuracy:
                    newValue += _modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    newValue *= _modificator;
                    break;
                case Buff_Type.IncomingDamageReduction:
                    newValue /= _modificator;
                    break;

                case Buff_Type.Attack:
                case Buff_Type.Health:
                case Buff_Type.Accuracy:
                case Buff_Type.Movement:
                case Buff_Type.Evasion:
                default:
                    throw new ArgumentException("Not incoming buff type");
            }

            return newValue;
        }
    }
}