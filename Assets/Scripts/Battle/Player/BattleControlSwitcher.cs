using AI;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BattleControlSwitcher : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private BattleMapSelector PlayerSelector;
    [SerializeField]
    private Button _endTurnButton;
    [SerializeField]
    private CooldownTimer _roundCounter;
    [SerializeField]
    private AIRunner _nonPlayerController;

    [Header("Settings")]
    [SerializeField]
    private bool _dontTouchPlayerSelector;
    [SerializeField]
    private bool _lockTurnButtonOnAITurn;

    private BattleLoopManager _battleManager;
    private IBattleContext _battleContext;
    //private bool _isTurnRunning;

    [Inject]
    private void Construct(BattleLoopManager battleManager, IBattleContext battleContext)
    {
        _battleManager = battleManager;
        _battleContext = battleContext;
        _battleContext.NewTurnStarted -= OnNewTurn;
        _battleContext.NewTurnStarted += OnNewTurn;
        _endTurnButton.onClick.RemoveListener(OnEndTurnButtonPressed);
        _endTurnButton.onClick.AddListener(OnEndTurnButtonPressed);
    }

    private async void OnNewTurn(IBattleContext battleContext)
    {
        if (!isActiveAndEnabled) return;
        //if (_isTurnRunning)
        //    throw new System.InvalidOperationException("How about \"F*ck you\"?");
        //_isTurnRunning = true;
        var roundColor = battleContext.ActiveSide switch
        {
            BattleSide.Player => Color.Lerp(Color.green, Color.white, 0.5f),
            BattleSide.Allies => Color.Lerp(Color.cyan, Color.white, 0.2f),
            BattleSide.Enemies => Color.Lerp(Color.red, Color.white, 0.5f),
            BattleSide.Others => Color.Lerp(Color.black, Color.white, 0.5f),
            BattleSide.NoSide => Color.Lerp(Color.black, Color.white, 0.0f),
            _ => throw new System.NotSupportedException(),
        };
        if (battleContext.EntitiesBank.GetActiveEntities(battleContext.ActiveSide).Length > 0)
            _roundCounter.SetValue(battleContext.CurrentRound, roundColor);

        if (battleContext.ActiveSide == BattleSide.Player)
        {
            if (!_dontTouchPlayerSelector)
                PlayerSelector.enabled = true;
            _endTurnButton.interactable = true;
        }
        else
        {
            if (!_dontTouchPlayerSelector)
                PlayerSelector.enabled = false;
            if (_lockTurnButtonOnAITurn)
                _endTurnButton.interactable = false;
            if (battleContext.ActiveSide != BattleSide.NoSide)
                await _nonPlayerController.Run(battleContext.ActiveSide);
            if (battleContext.HasExecutingTasks)
                await UniTask.WaitUntil(() => !battleContext.HasExecutingTasks);
            _battleManager.StartNextTurn();
        }
        //_isTurnRunning = false;
    }

    private void OnEndTurnButtonPressed()
    {
        if (!isActiveAndEnabled) return;
        _battleManager.StartNextTurn();
    }

    private void OnAbilityStarted()
    {
        if (_battleContext.ActiveSide == BattleSide.Player)
            _endTurnButton.interactable = false;
    }

    private void OnAbilityCompleted()
    {
        if (_battleContext.ActiveSide == BattleSide.Player)
            _endTurnButton.interactable = true;
    }

    private void Awake()
    {
        PlayerSelector.AbilityExecutionStarted -= OnAbilityStarted;
        PlayerSelector.AbilityExecutionCompleted -= OnAbilityCompleted;
        PlayerSelector.AbilityExecutionStarted += OnAbilityStarted;
        PlayerSelector.AbilityExecutionCompleted += OnAbilityCompleted;
    }
}
