using Ability;
using Ability.AbilityEffects;

public class AbilityBuilder
{
    public AbilityView Create(AbilityInfo abilityInfo)
    {
        IAbility ability = null;
        if (abilityInfo.HasAreaEffect)
        {
            ability = AddEffects(abilityInfo.AreaEffects, ability);
            ability = new AreaAbility(ability, abilityInfo.AreaRadius);
        }
        if (abilityInfo.HasTargetEffect)
        {
            ability = AddEffects(abilityInfo.TargetEffects, ability);
        }
        ability = new TargetAbility(ability, abilityInfo.Distance, !abilityInfo.HasTarget);

        return new AbilityView(ability, abilityInfo);
    }

    private static IAbility AddEffects(AbilityEffect[] effects, IAbility ability)
    {
        for (var i = effects.Length - 1; i >= 0; i--)
        {
            var effect = effects[i];
            switch (effect.Type)
            {
                case AbilityEffectType.Damage:
                    ability = new DamageAbility(ability, effect.Value);
                    break;
                case AbilityEffectType.Heal:
                    ability = new HealAbility(ability, effect.Value);
                    break;
                case AbilityEffectType.Move:
                    ability = new MoveAbility(ability);
                    break;
            }
        }

        return ability;
    }
}