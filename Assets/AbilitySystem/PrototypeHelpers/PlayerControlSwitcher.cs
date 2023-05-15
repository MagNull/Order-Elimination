using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PlayerControlSwitcher : MonoBehaviour
{
    [SerializeField]
    private BattleMapSelector PlayerSelector;
    [SerializeField]
    private Button _endTurnButton;
    [SerializeField]
    private bool _dontTouchPlayerSelector;
    private BattleLoopManager _battleManager;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _battleManager = objectResolver.Resolve<BattleLoopManager>();
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
            _endTurnButton.interactable = false;
            //wait for AI
        }
    }

    private void OnEndTurnButtonPressed()
    {
        if (!isActiveAndEnabled) return;
        if (_battleManager.ActiveSide != BattleSide.Player) return;
        _battleManager.StartNextTurn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 0.25f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
            _battleManager.StartNextTurn();
    }
}
