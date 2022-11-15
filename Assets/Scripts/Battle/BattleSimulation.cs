using System.Collections.Generic;
using UnityEngine;
using System;
using CharacterAbility;
using OrderElimination;
using OrderElimination.BattleMap;

//TODO(Илья): Refactor interaction with abilities. Decompose BattlSimualtion
public class BattleSimulation : MonoBehaviour
{
    public static event Action RoundStarted;
    public static event Action PlayerTurnEnd;

    public static event Action<BattleOutcome> BattleEnded;

    [SerializeField]
    private BattleMapDirector _battleMapDirector;
    [SerializeField]
    private CharacterArrangeDirector _characterArrangeDirector;
    [SerializeField]
    private AbilityViewBinder _abilityViewBinder;

    [SerializeField]
    private AbilityButton[] _abilityButtons;
    [SerializeField]
    private AbilityPanel _abilityPanel;

    private BattleObjectSide _currentTurn;
    private BattleOutcome _outcome;

    private bool _isBattleEnded = false;
    private bool _isTurnChanged = true;

    private List<BattleCharacter> _characters;

    public void Start()
    {
        _characters = new List<BattleCharacter>();

        InitializeBattlefield();
    }

    public void Update()
    {
        if (_outcome != BattleOutcome.Neither)
        {
            // По завершении сражения событие отправляется единожды
            if (!_isBattleEnded)
            {
                BattleEnded?.Invoke(_outcome);
                _isBattleEnded = true;
                Debug.LogFormat("Сражение завершено - победил {0}", _outcome == BattleOutcome.Victory ? "игрок" : "ИИ");
            }
        }
        else
        {
            if (_currentTurn == BattleObjectSide.Ally)
            {
                // Событие начала хода игрока отправляется один раз для каждого хода
                if (_isTurnChanged)
                {
                    RoundStarted?.Invoke();
                    _isTurnChanged = false;
                    Debug.Log("Начался ход игрока");
                }
                else
                {
                    // ?
                }
            }
            else if (_currentTurn == BattleObjectSide.Enemy)
            {
                if (_isTurnChanged)
                {
                    PlayerTurnEnd?.Invoke();
                    _isTurnChanged = false;
                    Debug.Log("Начался ход противника");
                }

                // Действия ИИ
            }
        }
    }

    public void CheckBattleOutcome()
    {
        bool isThereAnyAliveAlly = false;
        bool isThereAnyAliveEnemy = false;

        foreach (var unit in _characters)
        {
            if (unit.Stats.Health > 0)
            {
                if (unit.Side == BattleObjectSide.Ally)
                {
                    isThereAnyAliveAlly = true;
                }

                if (unit.Side == BattleObjectSide.Enemy)
                {
                    isThereAnyAliveEnemy = true;
                }
            }
        }

        _outcome = !isThereAnyAliveAlly
            ? BattleOutcome.Defeat
            : (isThereAnyAliveEnemy ? BattleOutcome.Neither : BattleOutcome.Victory);
    }

    // Вызывается извне класса, например кнопкой на экране
    // Влияет на Update()
    public void EndTurn()
    {
        //TODO: todo выше
        _abilityPanel.ResetAbilityButtons();
        SwitchTurn();
        _isTurnChanged = true;
    }

    public void SwitchTurn() => _currentTurn =
        _currentTurn == BattleObjectSide.Ally ? BattleObjectSide.Enemy : BattleObjectSide.Ally;

    public void InitializeBattlefield()
    {
        _currentTurn = BattleObjectSide.Ally;
        _outcome = BattleOutcome.Neither;

        // Создаём поле боя
        _battleMapDirector.InitializeMap();

        // Расставляем юнитов
        _characterArrangeDirector.SetArrangementMap(_battleMapDirector.Map);
        _characters = _characterArrangeDirector.Arrange();

        _abilityViewBinder.BindAbilityButtons(_battleMapDirector.MapView, _abilityButtons, _currentTurn);
    }
}