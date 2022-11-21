using System;
using CharacterAbility.BuffEffects;
using UnityEngine;

namespace CharacterAbility
{
    public enum IncomingDebuffType
    {
        Attack,
        Accuracy
    }
    
    [Serializable]
    public class IncomingBuff : TickEffectBase
    {
        [SerializeField]
        private IncomingDebuffType _incomingDebuffType;
        [SerializeField]
        private int _modificator;

        public IncomingDebuffType DebuffType => _incomingDebuffType;

        public IncomingBuff(IncomingDebuffType incomingDebuffType, int duration, int modificator) : base(duration)
        {
            _incomingDebuffType = incomingDebuffType;
            _modificator = modificator;
        }

        public int GetModifiedValue(int value) => value + _modificator;
    }
}