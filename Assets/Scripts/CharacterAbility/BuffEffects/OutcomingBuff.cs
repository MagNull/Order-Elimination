using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public class OutcomingBuff : TickEffectBase
    {
        [SerializeField]
        private float _modificator;
        [SerializeField]
        private Buff_Type _outcomingBuffType;
        [SerializeField]
        private DamageType _damageType;
        [SerializeField]
        private ITickEffect[] _triggerEffects;

        public float Modificator => _modificator;

        public Buff_Type OutcomingBuffType => _outcomingBuffType;

        public DamageType DamageType => _damageType;

        public OutcomingBuff(bool isUnique, Buff_Type outcomingBuffType, float modificator, int duration, ITickEffectView view,
            ITickEffect[] triggerEffects) : base(duration, view, isUnique)
        {
            _outcomingBuffType = outcomingBuffType;
            _modificator = modificator;
            _triggerEffects = triggerEffects;
        }

        public DamageInfo GetModifiedInfo(DamageInfo info)
        {
            if (info.Target is not BattleCharacter battleCharacter)
                return info;

            var targetEffects = battleCharacter.AllEffects;
            if (!targetEffects.Any(effect => _triggerEffects.Any(tef => tef.Equals(effect))))
            {
                return info;
            }

            switch (OutcomingBuffType)
            {
                case Buff_Type.OutcomingAccuracy:
                    info.Accuracy += (int) Modificator;
                    break;
                case Buff_Type.OutcomingAttack:
                    info.Damage = Mathf.RoundToInt(info.Damage * Modificator);
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

        public override bool Equals(ITickEffect tickEffect)
        {
            return tickEffect is OutcomingBuff outcomingBuff && outcomingBuff.OutcomingBuffType == OutcomingBuffType
                                                             && Math.Abs(outcomingBuff.Modificator - Modificator) <
                                                             0.01f && outcomingBuff.DamageType == DamageType;
        }
    }
}