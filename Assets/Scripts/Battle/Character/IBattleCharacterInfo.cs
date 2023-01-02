using CharacterAbility;
using OrderElimination;
using UnityEngine;

public interface IBattleCharacterInfo
{
    public BattleStats GetBattleStats();
    public string GetName();
    public Sprite GetViewIcon();
    public Sprite GetViewAvatar();
    public AbilityInfo[] GetActiveAbilityInfos();
    
    public AbilityInfo[] GetPassiveAbilityInfos();
}
