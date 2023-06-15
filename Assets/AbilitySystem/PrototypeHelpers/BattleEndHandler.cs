using DefaultNamespace;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BattleEndHandler : MonoBehaviour
{
    private IReadOnlyEntitiesBank _entitiesBank;
    private TextEmitter _textEmitter;

    [Inject]
    private void Construct(
        IBattleContext battleContext, 
        IReadOnlyEntitiesBank entitiesBank, 
        TextEmitter textEmitter)
    {
        _entitiesBank = entitiesBank;
        _textEmitter = textEmitter;
        battleContext.BattleStarted -= StartTrackingBattle;
        battleContext.BattleStarted += StartTrackingBattle;
    }

    private void StartTrackingBattle(IBattleContext battleContext)
    {
        _entitiesBank.BankChanged -= OnEntitiesBankChanged;
        _entitiesBank.BankChanged += OnEntitiesBankChanged;
    }

    private void OnEntitiesBankChanged(IReadOnlyEntitiesBank bank)
    {
        if (bank.GetEntities(BattleSide.Enemies).Length == 0)
        {
            _textEmitter.Emit($"Победа людей.", Color.green, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
            _textEmitter.Emit($"Нажмите «Esc» для выхода.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
        }
        else if (bank.GetEntities(BattleSide.Player).Length == 0)
        {
            _textEmitter.Emit($"Победа монстров.", Color.red, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
            _textEmitter.Emit($"Нажмите «Esc» для выхода.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
        }
    }
}
