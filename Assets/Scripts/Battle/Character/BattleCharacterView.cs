using UnityEngine;
using System;
using CharacterAbility;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DG.Tweening;
using OrderElimination.Battle;
using TMPro;

public class BattleCharacterView : MonoBehaviour, IBattleObjectView
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

    public IBattleObject Model => _character;
    public AbilityView[] ActiveAbilitiesView => _activeAbilitiesView;
    public AbilityView[] PassiveAbilitiesView => _passiveAbilitiesView;
    public string CharacterName { get; private set; }
    public Sprite Icon { get; private set; }
    public Sprite AvatarFull { get; private set; }

    public bool IsSelected => _selected;

    public GameObject GameObject => gameObject;
    public LineRenderer FireLine => _fireLine;

    public event Action<IBattleObjectView> Disabled;
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

        switch (Model.Type)
        {
            case BattleObjectType.Ally:
                BattleSimulation.PlayerTurnStarted += OnTurnStart;
                _character.ActionBankChanged += OnActionBankChanged;
                break;
            case BattleObjectType.Enemy:
                BattleSimulation.EnemyTurnStarted += OnTurnStart;
                break;
        }

        _activeAbilitiesView = activeAbilitiesView;
        _passiveAbilitiesView = passiveAbilitiesView;
        CharacterName = characterName;
        Icon = avatarIcon;
        AvatarFull = avatarFull;

        HideAccuracy();
    }

    public void OnDamaged(TakeDamageInfo info)
    {
        if (info is {ArmorDamage: 0, HealthDamage: 0})
        {
            switch (info.CancelType)
            {
                case DamageCancelType.Miss:
                    EmmitText("Miss", Color.yellow);
                    break;
                case DamageCancelType.Dodge:
                    EmmitText("Dodge", Color.green);
                    break;
            }

            return;
        }

        var startColor = _renderer.color;
        _renderer.DOColor(Color.red, _damagedDuration / 2).onComplete += () =>
        {
            _renderer.DOColor(startColor, _damagedDuration / 2);
        };
        EmmitText((info.ArmorDamage + info.HealthDamage).ToString(), Color.red);
    }

    public void EmmitText(string text, Color color, float fontSize = -1)
    {
        _textEmitter.Emit(text, color, fontSize);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        Disabled?.Invoke(this);
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

    public void HideAccuracy()
    {
        _shootProbability.gameObject.SetActive(false);
        _shootProbability.text = "";
    }

    private async void OnDied(BattleCharacter battleCharacter)
    {
        for (var i = 0; i < _dieFadeTimes - 1; i++)
        {
            await _renderer.DOColor(Color.clear, _dieDuration / (_dieFadeTimes * 4)).AsyncWaitForCompletion();
            await _renderer.DOColor(Color.white, _dieDuration / (_dieFadeTimes * 4)).AsyncWaitForCompletion();
        }

        await _renderer.DOColor(Color.clear, _dieDuration / _dieFadeTimes * 2).AsyncWaitForCompletion();
        _renderer.gameObject.SetActive(false);
        Disable();
    }

    private async void OnActionBankChanged(ActionBank actionBank)
    {
        await UniTask.WaitUntil(() =>
        {
            Color color = _renderer.color;
            return color == Color.grey || color == Color.white;
        });
        _renderer.color = actionBank.AvailableActions.Count <= 0 ? Color.grey : Color.white;
    }

    private void OnDisable()
    {
        _character.Damaged -= OnDamaged;
        _character.Died -= OnDied;

        switch (Model.Type)
        {
            case BattleObjectType.Ally:
                BattleSimulation.PlayerTurnStarted -= OnTurnStart;
                _character.ActionBankChanged -= OnActionBankChanged;
                break;
            case BattleObjectType.Enemy:
                BattleSimulation.EnemyTurnStarted -= OnTurnStart;
                break;
        }

        _character.ClearTickEffects();

        Destroy(gameObject);
    }

    private void OnTurnStart()
    {
        Deselect();
        _character.OnTurnStart();
    }
}