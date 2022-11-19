using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using OrderElimination;
using OrderElimination.Battle;
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
        //TODO: Fix stats
        var character = new BattleCharacter(side, new BattleStats(info.GetBattleStats()), new SimpleDamageCalculation());
        battleCharacterView.GetComponent<PlayerTestScript>().SetSide(side);
        battleCharacterView.Init(character, CreateCharacterAbilities(info.GetAbilityInfos(), character));

        character.SetView(battleCharacterView);
        return character;
    }

    // new
    public List<BattleCharacter> CreatePlayerSquad(List<IBattleCharacterInfo> infos) =>
        CreateSquad(infos, BattleObjectSide.Ally, "Player");

    // new
    public List<BattleCharacter> CreateEnemySquad(List<IBattleCharacterInfo> infos) =>
        CreateSquad(infos, BattleObjectSide.Enemy, "Enemy");

    // new
    public List<BattleCharacter> CreateSquad(List<IBattleCharacterInfo> infos, BattleObjectSide side, string name)
    {
        var characters = new List<BattleCharacter>(infos.Count);
        for (var i = 0; i < infos.Count; i++)
        {
            characters.Add(Create(infos[i], side));
            characters.Last().GetView().name = name + " " + i;
        }

        return characters;
    }

    //TODO(ÑÀÍÎ): Move to another response object
    private AbilityView[] CreateCharacterAbilities(AbilityInfo[] abilityInfos, BattleCharacter caster)
    {
        var abilities = new AbilityView[abilityInfos.Length];
        for (int i = 0; i < abilityInfos.Length; i++)
        {
            abilities[i] = _abilityBuilder.Create(abilityInfos[i], caster);
        }

        return abilities;
    }
}