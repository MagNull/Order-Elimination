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
        public float Modificator { get; }
        public Buff_Type IncomingBuffType { get; }
        public DamageType DamageType { get; }

        public IncomingBuff(Buff_Type incomingBuffType, int duration, float modificator, ITickEffectView view,
            DamageType damageType = DamageType.None) : base(duration, view)
        {
            IncomingBuffType = incomingBuffType;
            Modificator = modificator;
            DamageType = damageType;
        }

        public DamageInfo GetModifiedInfo(DamageInfo info)
        {
            switch (IncomingBuffType)
            {
                case Buff_Type.IncomingAccuracy:
                    if (info.DamageType == DamageType)
                        info.Accuracy += (int)Modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    if (info.DamageType == DamageType)
                        info.Damage = Mathf.RoundToInt(info.Damage * Modificator);
                    break;
                case Buff_Type.IncomingDamageReduction:
                    if (info.DamageType == DamageType)
                        info.Damage = Mathf.RoundToInt(info.Damage / Modificator);
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
            Debug.Log(buffType + " " + IncomingBuffType);
            if (IncomingBuffType != buffType)
                return newValue;
            switch (IncomingBuffType)
            {
                case Buff_Type.IncomingAccuracy:
                    newValue += (int)Modificator;
                    break;
                case Buff_Type.IncomingDamageIncrease:
                    newValue = Mathf.RoundToInt(Modificator * newValue);
                    break;
                case Buff_Type.IncomingDamageReduction:
                    newValue = Mathf.RoundToInt(newValue / Modificator);
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