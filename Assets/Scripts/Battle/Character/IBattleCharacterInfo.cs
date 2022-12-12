using CharacterAbility;
using OrderElimination;
using UnityEngine;

public interface IBattleCharacterInfo
{
    public BattleStats GetBattleStats();
    public Sprite GetView();
    public AbilityInfo[] GetActiveAbilityInfos();
    
    public AbilityInfo[] GetPassiveAbilityInfos();
}
