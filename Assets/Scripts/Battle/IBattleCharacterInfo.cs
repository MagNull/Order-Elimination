using CharacterAbility;
using OrderElimination;

public interface IBattleCharacterInfo
{
    public BattleStats GetBattleStats();
    public BattleCharacterView GetView();
    public AbilityInfo[] GetAbilityInfos();
}
