using CharacterAbility;
using OrderElimination;
using UnityEngine;

public interface IBattleCharacterInfo
{
    public IReadOnlyBattleStats GetBattleStats();
    public string GetName();
    public Sprite GetViewIcon();
    public Sprite GetViewAvatar();
    public AbilityInfo[] GetActiveAbilityInfos();
    
    public AbilityInfo[] GetPassiveAbilityInfos();
}
