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

    private bool _selected = false;

    public BattleCharacter Model => _character;
    public AbilityView[] AbilityViews => _abilityViews;

    public bool Selected => _selected;

    public void Init(BattleCharacter character, AbilityView[] abilitiesView)
    {
        _character = character;
        _character.Damaged += OnDamaged;
        _character.Died += OnDied;
        BattleSimulation.RoundStarted += OnRoundStart;

        _abilityViews = abilitiesView;
    }

    private void OnDamaged(int armorDamage, int healthDamage)
    {
        if(armorDamage == 0 && healthDamage == 0)
        {
            Debug.Log("Miss" % Colorize.Yellow);
            return;
        }
        Debug.Log(gameObject.name + " get "+ (armorDamage + healthDamage) + " damage " % Colorize.Red );
    }

    public void SetImage(Sprite image) => _renderer.sprite = image;

    public void Select() => _selected = true;
    
    public void Deselect() => _selected = false;

    public void OnDied()
    {
        Debug.Log(gameObject.name + " died" % Colorize.DarkRed);
    }

    private void OnDisable()
    {
        _character.ClearOverEffects();
    }

    private void OnRoundStart()
    {
        Deselect();
        _character.OnTurnStart();
    }
}