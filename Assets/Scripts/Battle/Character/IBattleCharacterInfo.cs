using CharacterAbility;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Domain;
using System.Collections.Generic;
using UnityEngine;

//Deprecated
public interface IBattleCharacterInfo : IBattleCharacterData
{
    public IReadOnlyBattleStats GetBattleStats();
    public string Name { get; }
    public Sprite BattleIcon { get; }
    public Sprite Avatar { get; }
    public AbilityInfo[] GetActiveAbilityInfos();
    public AbilityInfo[] GetPassiveAbilityInfos();
}

//New
public interface IBattleCharacterData//Rename to IBattleCharacterInfo
{
    public string Name { get; }
    public Sprite BattleIcon { get; }
    public Sprite Avatar { get; }
    public ReadOnlyBaseStats BaseBattleStats { get; }

    public int CostValue { get; }

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
