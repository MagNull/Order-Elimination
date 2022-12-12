using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;
using VContainer;

public class BattleCharacterFactory : MonoBehaviour
{
    [SerializeField]
    private BattleMap _map;
    [SerializeField]
    private BattleCharacterView charPrefab;
    private AbilityFactory _abilityFactory;
    [SerializeField]
    private AbilityInfo _bite;

    [Inject]
    public void Construct(AbilityFactory abilityFactory)
    {
        _abilityFactory = abilityFactory;
    }

    public BattleCharacter Create(IBattleCharacterInfo info, BattleObjectSide side)
    {
        BattleCharacterView battleCharacterView = Instantiate(charPrefab);
        battleCharacterView.SetImage(info.GetView());
        //TODO: Fix stats
        var character =
            new BattleCharacter(side, new BattleStats(info.GetBattleStats()), new SimpleDamageCalculation());
        //TODO: Generation Enemy 
        if (side == BattleObjectSide.Enemy)
        {
            character = new EnemyDog(_map, _abilityFactory.CreateAbility(_bite, character),
                new BattleStats(info.GetBattleStats()), new SimpleDamageCalculation());
        }

        battleCharacterView.GetComponentInChildren<SpriteRenderer>().sprite = info.GetView();
        battleCharacterView.Init(character, CreateCharacterAbilities(info.GetActiveAbilityInfos(), character),
            CreateCharacterAbilities(info.GetPassiveAbilityInfos(), character));

        character.View = battleCharacterView.gameObject;
        return character;
    }

    public List<BattleCharacter> CreatePlayerSquad(List<IBattleCharacterInfo> infos) =>
        CreateSquad(infos, BattleObjectSide.Ally, "Player");

    public List<BattleCharacter> CreateEnemySquad(List<IBattleCharacterInfo> infos) =>
        CreateSquad(infos, BattleObjectSide.Enemy, "Enemy");

    private List<BattleCharacter> CreateSquad(List<IBattleCharacterInfo> infos, BattleObjectSide side, string name)
    {
        var characters = new List<BattleCharacter>(infos.Count);
        for (var i = 0; i < infos.Count; i++)
        {
            characters.Add(Create(infos[i], side));
            characters.Last().View.name = name + " " + i;
        }

        return characters;
    }

    //TODO(����): Move to another response object
    private AbilityView[] CreateCharacterAbilities(AbilityInfo[] abilityInfos, BattleCharacter caster)
    {
        var abilities = new AbilityView[abilityInfos.Length];
        for (int i = 0; i < abilityInfos.Length; i++)
        {
            abilities[i] = _abilityFactory.CreateAbilityView(abilityInfos[i], caster);
        }

        return abilities;
    }
}