using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BattleMap;
using UnityEngine;

namespace CharacterAbility
{
    public class PassiveAbility : Ability
    {
        private readonly PassiveAbilityParams.PassiveTriggerType _passiveTriggerType;

        public PassiveAbility(IBattleObject caster, PassiveAbilityParams.PassiveTriggerType passiveTriggerType,
            Ability nextAbility, BattleObjectSide filter, float probability) : base(caster, nextAbility, filter,
            probability)
        {
            _passiveTriggerType = passiveTriggerType;
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
                        Use(target, stats);
                        Debug.Log("Apply Damage Passive");
                    };
                    break;
                case PassiveAbilityParams.PassiveTriggerType.Movement:
                    target.Moved += (from, to) =>
                    {
                        Debug.Log("Apply Move passive");
                    };
                    break;
            }
        }
    }
}