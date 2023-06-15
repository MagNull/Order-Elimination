using CharacterAbility;
using OrderElimination;
using System;
using System.Collections.Generic;
using UnityEngine;

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
