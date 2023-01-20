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
        battleCharacterView.SetImage(info.GetViewIcon());
        BattleCharacter character;
        //TODO: Generation Enemy 
        if (side == BattleObjectSide.Enemy)
        {
            character = new EnemyDog(_map,
                new BattleStats(info.GetBattleStats()), new SimpleDamageCalculation());
            ((EnemyDog) character).SetDamageAbility(_abilityFactory.CreateAbility(_bite, character));
        }
        else
        {
            character = new BattleCharacter(side, new BattleStats(info.GetBattleStats()),
                new SimpleDamageCalculation());
        }

        battleCharacterView.GetComponentInChildren<SpriteRenderer>().sprite = info.GetViewIcon();
        battleCharacterView.Init(character,
            CreateCharacterAbilities(info.GetActiveAbilityInfos(), character, battleCharacterView),
            CreateCharacterAbilities(info.GetPassiveAbilityInfos(), character, battleCharacterView),
            info.GetName(),
            info.GetViewIcon(),
            info.GetViewAvatar());

        character.View = battleCharacterView.gameObject.GetComponent<IBattleObjectView>();
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
            characters.Last().View.GameObject.name = name + " " + i;
        }

        return characters;
    }

    //TODO(����): Move to another response object
    private AbilityView[] CreateCharacterAbilities(AbilityInfo[] abilityInfos, BattleCharacter caster,
        BattleCharacterView casterView)
    {
        var abilities = new AbilityView[abilityInfos.Length];
        for (int i = 0; i < abilityInfos.Length; i++)
        {
            abilities[i] = _abilityFactory.CreateAbilityView(abilityInfos[i], caster, casterView);
        }

        return abilities;
    }
}