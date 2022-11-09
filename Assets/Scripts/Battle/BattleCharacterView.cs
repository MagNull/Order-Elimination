using UnityEngine;
using System;
using CharacterAbility;

public class BattleCharacterView : MonoBehaviour
{
    [SerializeField]
    private BattleCharacter _character;
    private AbilityView[] _abilityViews;
    [SerializeField]
    private SpriteRenderer _renderer;

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

    public void SetImage(Sprite image) => _renderer.sprite = image;

    public void OnDied()
    {
        // yet empty
    }

    private void OnDisable()
    {
        _character.ClearTickEffects();
    }

    public void OnTurnStart()
    {
        throw new NotImplementedException();
    }
}