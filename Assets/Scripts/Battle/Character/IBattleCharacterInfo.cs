using CharacterAbility;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.MetaGame;
using System;
using System.Collections.Generic;
using UnityEngine;

//Deprecated
[Obsolete("Deprecated. Use " + nameof(IGameCharacterTemplate) + " instead.")]
public interface IBattleCharacterInfo : IGameCharacterTemplate
{
    public IReadOnlyBattleStats GetBattleStats();
    public string Name { get; }
    public Sprite BattleIcon { get; }
    public Sprite Avatar { get; }
    public AbilityInfo[] GetActiveAbilityInfos();
    public AbilityInfo[] GetPassiveAbilityInfos();
}

//New
public interface IGameCharacterTemplate//Rename to IBattleCharacterInfo
{
    public string Name { get; }
    public Sprite BattleIcon { get; }
    public Sprite Avatar { get; }
    public int CostValue { get; }

    public IReadOnlyGameCharacterStats GetBaseBattleStats();
    public ActiveAbilityBuilder[] GetActiveAbilities();
    public PassiveAbilityBuilder[] GetPassiveAbilities();
}

public interface IBattleStructureData
{
    public string Name { get; }
    public Sprite BattleIcon { get; }
    public GameObject VisualModel { get; }
    public float MaxHealth { get; }
    public IBattleObstacleSetup ObstacleSetup { get; }

    public PassiveAbilityBuilder[] GetPossesedAbilities();
}
