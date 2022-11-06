using UnityEngine;
using System;
using CharacterAbility;

public class BattleCharacterView : MonoBehaviour
{
    [SerializeField]
    private BattleCharacter _character;
    private AbilityView[] _abilityViews;

    public BattleCharacter Model => _character;
    public AbilityView[] AbilityViews => _abilityViews;

    public void Init(BattleCharacter character, AbilityView[] abilitiesView)
    {
        _character = character;
        _character.Damaged += OnDamaged;

        _abilityViews = abilitiesView;
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