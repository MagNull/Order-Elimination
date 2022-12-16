using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CharacterAbility;
using OrderElimination;
using OrderElimination.BattleMap;
using VContainer;
using UIManagement.Elements;

public class BattleSimulation : MonoBehaviour
{
    public static event Action RoundStarted;
    public static event Action PlayerTurnEnd;

    public static event Action<BattleOutcome> BattleEnded;

    private CharacterArrangeDirector _characterArrangeDirector;
    private BattleMapDirector _battleMapDirector;
    private AbilityViewBinder _abilityViewBinder;

    [SerializeField]
    private AbilityButton[] _abilityButtons;
    //TODO: Remove panel, use event instead call inside methods(like EndRound, BattleEnd)
    [SerializeField]
    private AbilityPanel _abilityPanel;
    [SerializeField]
    private CharacterBattleStatsPanel _characterStatsWindow;

    private BattleObjectSide _currentTurn;
    private BattleOutcome _outcome;

    private bool _isBattleEnded = false;
    private bool _isTurnChanged = true;

    private List<BattleCharacter> _characters;

    [Inject]
    private void Construct(CharacterArrangeDirector characterArrangeDirector, BattleMapDirector battleMapDirector)
    {
        _battleMapDirector = battleMapDirector;
        _characterArrangeDirector = characterArrangeDirector;
    }

    private void Awake()
    {
        _characters = new List<BattleCharacter>();
        _abilityViewBinder = new AbilityViewBinder();
    }

    public void Start()
    {
        InitializeBattlefield();
    }

    public void Update()
    {
        CheckBattleOutcome();
        if (_outcome != BattleOutcome.Neither)
        {
            // По завершении сражения событие отправляется единожды
            if (_isBattleEnded) return;
            BattleEnded?.Invoke(_outcome);
            _abilityPanel.ResetAbilityButtons();
            _isBattleEnded = true;
            Debug.LogFormat("Сражение завершено - победил {0}", _outcome == BattleOutcome.Victory ? "игрок" : "ИИ");
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
                    Debug.Log("Начался ход игрока" % Colorize.Green);
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
                    Debug.Log("Начался ход противника" % Colorize.Red);
                }

                // Действия ИИ
                var enemies = _characters
                    .Select(x => x)
                    .Where(x => x.Side == BattleObjectSide.Enemy);
                foreach (var enemy in enemies)
                {
                    enemy.PlayTurn();
                }

                EndTurn();
            }
        }
    }

    // Тестовые методы
    public void AwardPlayerVictory() => _outcome = BattleOutcome.Victory;
    public void AwardPlayerDefeat() => _outcome = BattleOutcome.Defeat;

    public void CheckBattleOutcome()
    {
        if (_outcome != BattleOutcome.Neither)
        {
            return;
        }

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
        //TODO затрагивает UI
        _characterStatsWindow.Bind(_battleMapDirector.MapView, _currentTurn);
    }
}