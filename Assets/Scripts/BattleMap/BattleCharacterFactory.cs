using CharacterAbility;
using UnityEngine;

public class BattleCharacterFactory : MonoBehaviour
{
    [SerializeField]
    private BattleCharacterView charPrefab;
    [SerializeField]
    private  AbilityBuilder _abilityBuilder;
    [SerializeField]
    private AbilityButton _abilityButton;

    public BattleCharacter Create(IBattleCharacterInfo info, CharacterSide side)
    {
        BattleCharacterView battleCharacterView = Instantiate(charPrefab);
        var character = new BattleCharacter(side, info.GetBattleStats());
        battleCharacterView.GetComponent<PlayerTestScript>().SetSide(side);
        battleCharacterView.Init(character, CreateAbilities(info.GetAbilityInfos(), character));
        
        character.SetView(battleCharacterView);
        battleCharacterView.BattleCharacterViewClicked += characterView =>
        {
            var move = characterView.AbilitiesView[0];
            _abilityButton.SetAbility(move);
        };
        return character;
    }

    private AbilityView[] CreateAbilities(AbilityInfo[] abilityInfos, BattleCharacter caster)
    {
        var abilities = new AbilityView[abilityInfos.Length];
        for(int i = 0; i < abilityInfos.Length; i++)
        {
            abilities[i] = _abilityBuilder.Create(abilityInfos[i], caster);
        }

        return abilities;
    }
}
