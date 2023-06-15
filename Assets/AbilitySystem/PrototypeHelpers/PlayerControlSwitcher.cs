using DefaultNamespace;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
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
    [Header("Settings")]
    [SerializeField]
    private bool _dontTouchPlayerSelector;
    [SerializeField]
    [Header("")]
    private bool _lockTurnButtonOnAITurn;

    private BattleLoopManager _battleManager;
    //private TextEmitter _textEmitter;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _battleManager = objectResolver.Resolve<BattleLoopManager>();
        //_textEmitter = objectResolver.Resolve<TextEmitter>();
        _battleManager.NewTurnStarted -= OnNewTurn;
        _battleManager.NewTurnStarted += OnNewTurn;
        _endTurnButton.onClick.RemoveListener(OnEndTurnButtonPressed);
        _endTurnButton.onClick.AddListener(OnEndTurnButtonPressed);
    }

    private void OnNewTurn()
    {
        if (!isActiveAndEnabled) return;
        if (_battleManager.ActiveSide == BattleSide.Player)
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
            //wait for AI
        }
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
