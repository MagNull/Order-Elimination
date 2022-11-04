using CharacterAbility;
using OrderElimination.BattleMap;
using UnityEngine;

public class BattleCharacterFactory : MonoBehaviour
{
    [SerializeField]
    private BattleCharacterView charPrefab;
    [SerializeField]
    private AbilityBuilder _abilityBuilder;
    [SerializeField]
    private AbilityButton _move;
    [SerializeField]
    private AbilityButton _damage;

    [SerializeField]
    private BattleMap _map;

    public BattleCharacter Create(IBattleCharacterInfo info, BattleObjectSide side)
    {
        BattleCharacterView battleCharacterView = Instantiate(charPrefab);
        var character = new BattleCharacter(side, info.GetBattleStats());
        battleCharacterView.GetComponent<PlayerTestScript>().SetSide(side);
        battleCharacterView.Init(character, CreateAbilities(info.GetAbilityInfos(), character));

        character.SetView(battleCharacterView);
        _map.CellSelected += cell =>
        {
            if (cell.GetObject() is NullBattleObject ||
                !cell.GetObject().GetView().TryGetComponent(out BattleCharacterView characterView))
                return;
            var move = characterView.AbilitiesView[0];
            var damage = characterView.AbilitiesView[1];
            _move.SetAbility(move);
            _damage.SetAbility(damage);
        };
        return character;
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