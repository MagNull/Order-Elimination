using UnityEngine;
using System;
using CharacterAbility;

public class BattleCharacterView : MonoBehaviour
{
    [SerializeField]
    private BattleCharacter _character;
    private AbilityView[] _abilitiesView;

    public BattleCharacter Model => _character;
    public AbilityView[] AbilitiesView => _abilitiesView;

    public void Init(BattleCharacter character, AbilityView[] abilitiesView)
    {
        _character = character;
        _character.Damaged += OnDamaged;

        _abilitiesView = abilitiesView;
    }

    public void OnDamaged(int damage)
    {
        Debug.Log(gameObject.name + " damaged");
    }

    public void OnDied()
    {
        // yet empty
    }

    public void OnTurnStart()
    {
        throw new NotImplementedException();
    }
}