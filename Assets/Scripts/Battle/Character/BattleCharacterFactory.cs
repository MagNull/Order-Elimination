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
    private IReadOnlyCharacterBank _characterBank;

    [Inject]
    public void Construct(AbilityFactory abilityFactory, IReadOnlyCharacterBank characterBank)
    {
        _characterBank = characterBank;
        _abilityFactory = abilityFactory;
    }

    public BattleCharacter Create(IBattleCharacterInfo info, BattleObjectType type)
    {
        BattleCharacterView battleCharacterView = Instantiate(charPrefab);
        battleCharacterView.SetImage(info.GetViewIcon());
        BattleCharacter character;
        //TODO: Generation Enemy 
        if (type == BattleObjectType.Enemy)
        {
            //TODO: Remove
            IDamageCalculation damageCalculation = new SimpleDamageCalculation();

            character = new RandomEnemyAI(_map,
                new BattleStats(info.GetBattleStats()), damageCalculation, _characterBank);
            List<AIAbility> abilitiesInfo = new List<AIAbility>();
            foreach (var activeAbilityInfo in info.GetActiveAbilityInfos().Skip(1))
            {
                var ability = _abilityFactory.CreateAbility(activeAbilityInfo, character);
                var aiInfo = new AIAbility
                (
                    activeAbilityInfo.ActiveParams.Distance,
                    activeAbilityInfo.ActiveParams.TargetType,
                    ability,
                    activeAbilityInfo.Name,
                    activeAbilityInfo.CoolDown,
                    activeAbilityInfo.StartCoolDown
                );
                abilitiesInfo.Add(aiInfo);
            }
            
            var moveAbility = _abilityFactory.CreateAbility(info.GetActiveAbilityInfos()[0], character);
            ((RandomEnemyAI) character).SetMoveAbility(moveAbility);

            ((RandomEnemyAI) character).SetAbilities(abilitiesInfo);
        }
        else
        {
            character = new BattleCharacter(type, new BattleStats(info.GetBattleStats()),
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
        CreateSquad(infos, BattleObjectType.Ally, "Player");

    public List<BattleCharacter> CreateEnemySquad(List<IBattleCharacterInfo> infos) =>
        CreateSquad(infos, BattleObjectType.Enemy, "Enemy");

    private List<BattleCharacter> CreateSquad(List<IBattleCharacterInfo> infos, BattleObjectType type, string name)
    {
        var characters = new List<BattleCharacter>(infos.Count);
        for (var i = 0; i < infos.Count; i++)
        {
            characters.Add(Create(infos[i], type));
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