using CharacterAbility;
using OrderElimination;

public class TestBattleCharacterInfo : IBattleCharacterInfo
{
    public AbilityInfo[] GetAbilityInfos()
    {
        return new AbilityInfo[] { };
    }

    public BattleStats GetBattleStats()
    {
        return new BattleStats();
    }

    public BattleCharacterView GetView()
    {
        throw new System.NotImplementedException();
    }
}
