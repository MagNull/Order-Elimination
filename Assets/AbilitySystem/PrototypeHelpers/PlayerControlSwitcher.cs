using DefaultNamespace;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private TextEmitter _textEmitter;

    private IReadOnlyEntitiesBank _entitiesBank;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _battleManager = objectResolver.Resolve<BattleLoopManager>();
        _textEmitter = objectResolver.Resolve<TextEmitter>();
        _entitiesBank = objectResolver.Resolve<IBattleContext>().EntitiesBank;
        _battleManager.NewTurnStarted -= OnNewTurn;
        _battleManager.NewTurnStarted += OnNewTurn;
        _endTurnButton.onClick.RemoveListener(OnEndTurnButtonPressed);
        _endTurnButton.onClick.AddListener(OnEndTurnButtonPressed);
    }

    private void OnNewTurn()
    {
        _entitiesBank.BankChanged -= OnEntitiesBankChanged;
        _entitiesBank.BankChanged += OnEntitiesBankChanged;
        if (!isActiveAndEnabled) return;
        if (_battleManager.ActiveSide == BattleSide.Player)
        {
            if (!_dontTouchPlayerSelector)
                PlayerSelector.Enable();
            //_endTurnButton.interactable = true;
        }
        else
        {
            if (!_dontTouchPlayerSelector)
                PlayerSelector.Disable();
            //_endTurnButton.interactable = false;
            //wait for AI
        }
    }

    private void OnEndTurnButtonPressed()
    {
        if (!isActiveAndEnabled) return;
        //if (_battleManager.ActiveSide != BattleSide.Player) return;
        _battleManager.StartNextTurn();
    }

    private void OnEntitiesBankChanged(IReadOnlyEntitiesBank bank)
    {
        if (bank.GetEntities(BattleSide.Enemies).Length == 0)
        {
            _textEmitter.Emit($"������ �����.", Color.green, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
            _textEmitter.Emit($"������� �Esc� ��� ������.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
        }
        else if (bank.GetEntities(BattleSide.Player).Length == 0)
        {
            _textEmitter.Emit($"������ ��������.", Color.red, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
            _textEmitter.Emit($"������� �Esc� ��� ������.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
        }
    }

    //ToRemove
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _battleManager.StartNextTurn();
    }
}
