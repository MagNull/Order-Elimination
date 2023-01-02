using UnityEngine;
using System;
using CharacterAbility;
using OrderElimination.Battle;

public class BattleCharacterView : MonoBehaviour
{
    [SerializeField]
    private BattleCharacter _character;
    private AbilityView[] _activeAbilitiesView;
    private AbilityView[] _passiveAbilitiesView;
    [SerializeField]
    private SpriteRenderer _renderer;

    private bool _selected = false;

    public BattleCharacter Model => _character;
    public AbilityView[] ActiveAbilitiesView => _activeAbilitiesView;
    public AbilityView[] PassiveAbilitiesView => _passiveAbilitiesView;
    public Sprite Icon { get; private set; }
    public Sprite AvatarFull { get; private set; }

    public bool IsSelected => _selected;

    public static event Action<BattleCharacterView> Selected;
    public static event Action<BattleCharacterView> Deselected;

    public void Init(BattleCharacter character, AbilityView[] activeAbilitiesView, AbilityView[] passiveAbilitiesView, Sprite avatarIcon, Sprite avatarFull)
    {
        Selected = null;
        Deselected = null;
        _passiveAbilitiesView = passiveAbilitiesView;
        _character = character;
        _character.Damaged += OnDamaged;
        _character.Died += OnDied;
        BattleSimulation.RoundStarted += OnRoundStart;

        _activeAbilitiesView = activeAbilitiesView;
        _passiveAbilitiesView = passiveAbilitiesView;
        Icon = avatarIcon;
        AvatarFull = avatarFull;
    }

    private void OnDamaged(TakeDamageInfo info)
    {
        if(info is {ArmorDamage: 0, HealthDamage: 0})
        {
            switch (info.CancelType)
            {
                case DamageCancelType.Miss:
                    Debug.Log("Miss " % Colorize.Yellow + gameObject.name);
                    break;
                case DamageCancelType.Dodge:
                    Debug.Log("Dodge " % Colorize.Green + gameObject.name);
                    break;
            }

            return;
        }
        Debug.Log(gameObject.name + " get "+ (info.ArmorDamage + info.HealthDamage) + " damage " % Colorize.Red );
    }

    public void SetImage(Sprite image) => _renderer.sprite = image;

    public void Select()
    {
        _selected = true;
        Selected?.Invoke(this);
    }

    public void Deselect()
    {
        _selected = false;
        Deselected?.Invoke(this);
    }

    private void OnDied(BattleCharacter battleCharacter)
    {
        Debug.Log(gameObject.name + " died" % Colorize.DarkRed);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        _character.Damaged -= OnDamaged;
        _character.Died -= OnDied;
        BattleSimulation.RoundStarted -= OnRoundStart;
        _character.ClearTickEffects();
    }

    private void OnRoundStart()
    {
        Deselect();
        _character.OnTurnStart();
    }
}