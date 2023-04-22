using CharacterAbility;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.OuterComponents;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleCharacterInfo : IBattleEntityInfo
{
    public IReadOnlyBattleStats GetBattleStats();
    public string Name { get; }
    public Sprite BattleIcon { get; }
    public Sprite Avatar { get; }
    public AbilityInfo[] GetActiveAbilityInfos();
    public AbilityInfo[] GetPassiveAbilityInfos();
}

public interface IBattleEntityInfo
{
    //View
    public string Name { get; }
    public Sprite BattleIcon { get; }
    //View
    public ReadOnlyBaseStats BaseStats { get; }
    //public EntityType EntityType { get; }
    public AbilityBuilderData[] GetActiveAbilities();
    public AbilityBuilderData[] GetPassiveAbilities();
}
