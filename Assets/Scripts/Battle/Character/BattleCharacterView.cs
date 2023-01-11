using UnityEngine;
using System;
using CharacterAbility;
using DefaultNamespace;
using DG.Tweening;
using OrderElimination.Battle;
using TMPro;

public class BattleCharacterView : MonoBehaviour
{
    [SerializeField]
    private BattleCharacter _character;
    private AbilityView[] _activeAbilitiesView;
    private AbilityView[] _passiveAbilitiesView;
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private TextMeshProUGUI _shootProbability;
    [SerializeField]
    private LineRenderer _fireLine;
    [Header("Reactions")]
    [SerializeField]
    private TextEmitter _textEmitter;
    [SerializeField]
    private float _damagedDuration;
    [Header("Die")]
    [SerializeField]
    private float _dieDuration;
    [SerializeField]
    private float _dieFadeTimes;

    private bool _selected = false;

    public BattleCharacter Model => _character;
    public AbilityView[] ActiveAbilitiesView => _activeAbilitiesView;
    public AbilityView[] PassiveAbilitiesView => _passiveAbilitiesView;
    public string CharacterName { get; private set; }
    public Sprite Icon { get; private set; }
    public Sprite AvatarFull { get; private set; }

    public bool IsSelected => _selected;

    public LineRenderer FireLine => _fireLine;

    public static event Action<BattleCharacterView> Selected;
    public static event Action<BattleCharacterView> Deselected;

    public void Init(BattleCharacter character, AbilityView[] activeAbilitiesView, AbilityView[] passiveAbilitiesView,
        string characterName, Sprite avatarIcon, Sprite avatarFull)
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
        CharacterName = characterName;
        Icon = avatarIcon;
        AvatarFull = avatarFull;

        HideProbability();
    }

    private void OnDamaged(TakeDamageInfo info)
    {
        if (info is {ArmorDamage: 0, HealthDamage: 0})
        {
            switch (info.CancelType)
            {
                case DamageCancelType.Miss:
                    _textEmitter.Emit("Miss", Color.yellow);
                    break;
                case DamageCancelType.Dodge:
                    _textEmitter.Emit("Dodge", Color.green);
                    break;
            }

            return;
        }

        _renderer.DOColor(Color.red, _damagedDuration / 2).onComplete += () =>
        {
            _renderer.DOColor(Color.white, _damagedDuration / 2);
        };
        _textEmitter.Emit((info.ArmorDamage + info.HealthDamage).ToString(), Color.red);
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

    public void ShowAccuracy(int probability)
    {
        _shootProbability.gameObject.SetActive(true);
        _shootProbability.text = probability.ToString();
    }

    public void HideProbability()
    {
        _shootProbability.gameObject.SetActive(false);
        _shootProbability.text = "";
    }

    private async void OnDied(BattleCharacter battleCharacter)
    {
        Debug.Log(gameObject.name + " died" % Colorize.DarkRed);
        for(var i = 0; i < _dieFadeTimes - 1; i++)
        {
            await _renderer.DOColor(Color.clear, _dieDuration / (_dieFadeTimes * 4)).AsyncWaitForCompletion();
            await _renderer.DOColor(Color.white, _dieDuration / (_dieFadeTimes * 4)).AsyncWaitForCompletion();
        }
        await _renderer.DOColor(Color.clear, _dieDuration / _dieFadeTimes * 2).AsyncWaitForCompletion();
        _renderer.gameObject.SetActive(false);
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