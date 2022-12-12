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
        private readonly Buff_Type _incomingBuffType;
        private readonly DamageType _damageType;

        public IncomingBuff(Buff_Type incomingBuffType, int duration, int modificator,
            DamageType damageType = DamageType.None) : base(duration)
        {
            _incomingBuffType = incomingBuffType;
            _modificator = modificator;
            _damageType = damageType;
        }

        public DamageInfo GetModifiedValue(DamageInfo info)
        {
            switch (_incomingBuffType)
            {
                case Buff_Type.IncomingAccuracy:
                    info.Accuracy += _modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    info.Damage *= _modificator;
                    break;
                case Buff_Type.IncomingDamageReduction:
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
    }
}