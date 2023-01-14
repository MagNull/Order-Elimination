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
        protected float _modificator;
        [SerializeField]
        private readonly Buff_Type _incomingBuffType;
        private readonly DamageType _damageType;

        public IncomingBuff(Buff_Type incomingBuffType, int duration, float modificator, ITickEffectView view,
            DamageType damageType = DamageType.None) : base(duration, view)
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
                        info.Accuracy += (int)_modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    if (info.DamageType == _damageType)
                        info.Damage = Mathf.RoundToInt(info.Damage * _modificator);
                    break;
                case Buff_Type.IncomingDamageReduction:
                    if (info.DamageType == _damageType)
                        info.Damage = Mathf.RoundToInt(info.Damage / _modificator);
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
                    newValue += (int)_modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    newValue = Mathf.RoundToInt(_modificator * newValue);
                    break;
                case Buff_Type.IncomingDamageReduction:
                    newValue = Mathf.RoundToInt(newValue / _modificator);
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