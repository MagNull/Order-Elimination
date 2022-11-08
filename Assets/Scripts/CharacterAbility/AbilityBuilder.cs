using System;
using CharacterAbility.AbilityEffects;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityBuilder
    {
        [SerializeField]
        private BattleMapView _battleMapView;

        public AbilityView Create(AbilityInfo abilityInfo, BattleCharacter caster)
        {
            Ability ability = null;
            if (abilityInfo.HasAreaEffect)
            {
                ability = AddEffects(abilityInfo.AreaEffects, ability, caster);
                ability = new AreaAbility(caster, ability, abilityInfo.AreaRadius);
            }

            if (abilityInfo.HasTargetEffect)
            {
                ability = AddEffects(abilityInfo.TargetEffects, ability, caster);
            }

            ability = new TargetAbility(caster, ability,
                abilityInfo.DistanceFromMovement ? caster.Stats.Movement : abilityInfo.Distance,
                abilityInfo.TargetType == TargetType.Self);

            return new AbilityView(caster, ability, abilityInfo, _battleMapView);
        }

        private static Ability AddEffects(AbilityEffect[] effects, Ability ability, BattleCharacter caster)
        {
            for (var i = effects.Length - 1; i >= 0; i--)
            {
                var effect = effects[i];
                switch (effect.Type)
                {
                    case AbilityEffectType.Damage:
                        ability = new DamageAbility(caster, ability, effect.Amounts, effect.AttackScale);
                        break;
                    case AbilityEffectType.Heal:
                        ability = new HealAbility(caster, ability, effect.Amounts, effect.AttackScale);
                        break;
                    case AbilityEffectType.Move:
                        ability = new MoveAbility(caster, ability);
                        break;
                    case AbilityEffectType.Modificator:
                        ability = new ModificatorAbility(caster, ability, effect.Modification, effect.ModificationValue);
                        break;
                }
            }

            return ability;
        }
    }
}