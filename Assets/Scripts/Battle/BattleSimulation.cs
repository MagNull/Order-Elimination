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
    private BattleCharacterFactory _characterFactory;
    [SerializeField]
    private BattleMap _map;
    [SerializeField]
    private Character _characterInfo;
    [SerializeField]
    private AbilityButton[] _abilityButtons;
    [SerializeField]
    private AbilityCancel _abilityCancel;

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
            if (_currentTurn == BattleObjectSide.Player)
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
                if (unit.Side == BattleObjectSide.Player)
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
        _abilityCancel.ResetAbilityButtons();
        SwitchTurn();
        _isTurnChanged = true;
    }

    public void SwitchTurn() => _currentTurn =
        _currentTurn == BattleObjectSide.Player ? BattleObjectSide.Enemy : BattleObjectSide.Player;

    public void InitializeBattlefield()
    {
        _currentTurn = BattleObjectSide.Player;
        _outcome = BattleOutcome.Neither;

        _map.Init();
        ArrangeCharacters(GetTestSquad(2), GetTestSquad(3));
        BindAbilityButtons();
    }

    private void BindAbilityButtons()
    {
        _map.CellClicked += cell =>
        {
            if (cell.GetObject() is NullBattleObject ||
                !cell.GetObject().GetView().TryGetComponent(out BattleCharacterView characterView) ||
                characterView.Model.Side != BattleObjectSide.Player
                || _currentTurn != BattleObjectSide.Player)
                return;
            for (var i = 0; i < characterView.AbilityViews.Length; i++)
            {
                _abilityButtons[i].CancelAbilityCast();
                _abilityButtons[i].SetAbility(characterView.AbilityViews[i]);
            }
            //TODO: Автовыбор перемещения независимо от порядка
            _abilityButtons[0].OnClicked();
        };
    }

    // Расставляет юнитов на игровом поле
    private void ArrangeCharacters(IBattleCharacterInfo[] alliesInfo, IBattleCharacterInfo[] enemiesInfo)
    {
        BattleCharacter[] playerSquad = _characterFactory.CreatePlayerSquad(alliesInfo);
        for (int i = 0; i < playerSquad.Length; i++)
        {
            _characters.Add(playerSquad[i]);
            _map.SetCell(0, 2 * i, playerSquad[i]);
        }

        BattleCharacter[] enemySquad = _characterFactory.CreateEnemySquad(enemiesInfo);
        for (int i = 0; i < enemySquad.Length; i++)
        {
            _characters.Add(enemySquad[i]);
            _map.SetCell(_map.Width - 1, i, enemySquad[i]);
        }
    }

    public IBattleCharacterInfo[] GetTestSquad(int squadMembersCount)
    {
        var result = new IBattleCharacterInfo[squadMembersCount];
        for (int i = 0; i < squadMembersCount; i++)
        {
            result[i] = _characterInfo;
        }

        return result;
    }
}