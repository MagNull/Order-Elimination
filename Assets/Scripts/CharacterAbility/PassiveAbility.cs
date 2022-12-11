using System;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BattleMap;
using UnityEngine;

namespace CharacterAbility
{
    public class PassiveAbility : Ability
    {
        private readonly PassiveAbilityParams.PassiveTriggerType _passiveTriggerType;
        private readonly Ability _effects;

        public PassiveAbility(IBattleObject caster, PassiveAbilityParams.PassiveTriggerType passiveTriggerType,
            Ability effects, BattleObjectSide filter, float probability) : base(caster, effects, filter,
            probability)
        {
            _passiveTriggerType = passiveTriggerType;
            _effects = effects;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            switch (_passiveTriggerType)
            {
                case PassiveAbilityParams.PassiveTriggerType.Damage:
                    target.Damaged += (armorDamage, healthDamage, cancelType) =>
                    {
                        if (cancelType != DamageCancelType.None)
                            return;
                        _effects?.Use(target, stats);
                        Debug.Log("Apply Damage Passive");
                    };
                    break;
                case PassiveAbilityParams.PassiveTriggerType.Movement:
                    target.Moved += (from, to) =>
                    {
                        if(to.GetObject() is EnvironmentObject)
                            _effects?.Use(target, stats);
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}