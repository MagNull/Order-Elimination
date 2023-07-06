using DefaultNamespace;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PlayerControlSwitcher : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private BattleMapSelector PlayerSelector;
    [SerializeField]
    private Button _endTurnButton;
    [SerializeField]
    private CooldownTimer _roundCounter;
    [Header("Settings")]
    [SerializeField]
    private bool _dontTouchPlayerSelector;
    [SerializeField]
    private bool _lockTurnButtonOnAITurn;

    private BattleLoopManager _battleManager;
    private IBattleContext _battleContext;
    //private TextEmitter _textEmitter;

    [Inject]
    private void Construct(BattleLoopManager battleManager, IBattleContext battleContext)
    {
        _battleManager = battleManager;
        _battleContext = battleContext;
        //_textEmitter = objectResolver.Resolve<TextEmitter>();
        _battleContext.NewTurnStarted -= OnNewTurn;
        _battleContext.NewTurnStarted += OnNewTurn;
        _endTurnButton.onClick.RemoveListener(OnEndTurnButtonPressed);
        _endTurnButton.onClick.AddListener(OnEndTurnButtonPressed);
    }

    private void OnNewTurn(IBattleContext battleContext)
    {
        if (!isActiveAndEnabled) return;
        if (battleContext.ActiveSide == BattleSide.Player)
        {
            if (!_dontTouchPlayerSelector)
                PlayerSelector.Enable();
            _endTurnButton.interactable = true;
        }
        else
        {
            if (!_dontTouchPlayerSelector)
                PlayerSelector.Disable();
            if (_lockTurnButtonOnAITurn)
                _endTurnButton.interactable = false;
        }
        var roundColor = battleContext.ActiveSide switch
        {
            BattleSide.Player => Color.Lerp(Color.green, Color.white, 0.5f),
            BattleSide.Allies => Color.Lerp(Color.cyan, Color.white, 0.2f),
            BattleSide.Enemies => Color.Lerp(Color.red, Color.white, 0.5f),
            BattleSide.Others => Color.Lerp(Color.black, Color.white, 0.5f),
            _ => throw new System.NotSupportedException(),
        };
        if (battleContext.EntitiesBank.GetEntities(battleContext.ActiveSide).Length > 0)
            _roundCounter.SetValue(battleContext.CurrentRound, roundColor);
    }

    private void OnEndTurnButtonPressed()
    {
        if (!isActiveAndEnabled) return;
        _battleManager.StartNextTurn();
    }

    //ToRemove
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _battleManager.StartNextTurn();
    }
}
