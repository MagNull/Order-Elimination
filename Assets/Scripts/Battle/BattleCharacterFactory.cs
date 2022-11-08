using CharacterAbility;
using OrderElimination.Battle;
using OrderElimination.BattleMap;
using UnityEngine;

public class BattleCharacterFactory : MonoBehaviour
{
    [SerializeField]
    private BattleCharacterView charPrefab;
    [SerializeField]
    private AbilityBuilder _abilityBuilder;


    [SerializeField]
    private BattleMap _map;

    public BattleCharacter Create(IBattleCharacterInfo info, BattleObjectSide side)
    {
        BattleCharacterView battleCharacterView = Instantiate(charPrefab);
        battleCharacterView.SetImage(info.GetView());
        var character = new BattleCharacter(side, info.GetBattleStats(), new SimpleDamageCalculation());
        battleCharacterView.GetComponent<PlayerTestScript>().SetSide(side);
        battleCharacterView.Init(character, CreateAbilities(info.GetAbilityInfos(), character));

        character.SetView(battleCharacterView);
        return character;
    }

    // new
    public BattleCharacter[] CreatePlayerSquad(IBattleCharacterInfo[] infos) => CreateSquad(infos, BattleObjectSide.Player);
    // new
    public BattleCharacter[] CreateEnemySquad(IBattleCharacterInfo[] infos) => CreateSquad(infos, BattleObjectSide.Enemy);

    // new
    public BattleCharacter[] CreateSquad(IBattleCharacterInfo[] infos, BattleObjectSide side)
    {
        var characters = new BattleCharacter[infos.Length];
        for (var i = 0; i < infos.Length; i++)
        {
            characters[i] = Create(infos[i], side);
        }
        return characters;
    }

    private AbilityView[] CreateAbilities(AbilityInfo[] abilityInfos, BattleCharacter caster)
    {
        var abilities = new AbilityView[abilityInfos.Length];
        for (int i = 0; i < abilityInfos.Length; i++)
        {
            abilities[i] = _abilityBuilder.Create(abilityInfos[i], caster);
        }

        return abilities;
    }
}