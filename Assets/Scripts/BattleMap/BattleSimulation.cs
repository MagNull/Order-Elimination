using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OrderElimination;

public class BattleSimulation : MonoBehaviour
{
    public static event Action OnPlayerTurnStart;
    public static event Action OnPlayerTurnEnd;

    public static event Action<BattleOutcome> OnBattleEnded;

    [SerializeField] private BattleCharacterFactory _characterFactory;
    [SerializeField] private BattleMap _map;
    [SerializeField] private Character _characterInfo;

    private BattleObjectSide currentTurn;
    private BattleOutcome outcome;

    private bool isBattleEnded = false;
    private bool isTurnChanged = true;

    private List<BattleCharacter> _characters;

    public void Start()
    {
        _characters = new List<BattleCharacter>();

        InitializeBattlefield();
    }

    public void Update()
    {
        if (outcome != BattleOutcome.Neither)
        {
            // По завершении сражения событие отправляется единожды
            if (!isBattleEnded)
            {
                OnBattleEnded?.Invoke(outcome);
                isBattleEnded = true;
                Debug.LogFormat("Сражение завершено - победил {0}", outcome == BattleOutcome.Victory ? "игрок" : "ИИ");
            }
        }
        else
        {
            if (currentTurn == BattleObjectSide.Player)
            {
                // Событие начала хода игрока отправляется один раз для каждого хода
                if (isTurnChanged)
                {
                    OnPlayerTurnStart?.Invoke();
                    isTurnChanged = false;
                    Debug.Log("Начался ход игрока");
                }
                else
                {
                    // ?
                }
            }
            else if (currentTurn == BattleObjectSide.Enemy)
            {
                if (isTurnChanged)
                {
                    OnPlayerTurnEnd?.Invoke();
                    isTurnChanged = false;
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

        for (int i = 0; i < _characters.Count; i++)
        {
            BattleCharacter unit = _characters[i];
            if (unit.Health > 0)
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

        outcome = !isThereAnyAliveAlly ? BattleOutcome.Defeat : (isThereAnyAliveEnemy ? BattleOutcome.Neither : BattleOutcome.Victory); 
    }

    // Вызывается извне класса, например кнопкой на экране
    // Влияет на Update()
    public void EndTurn()
    {
        SwitchTurn();
        isTurnChanged = true;
    }

    public void SwitchTurn() => currentTurn = currentTurn == BattleObjectSide.Player ? BattleObjectSide.Enemy : BattleObjectSide.Player;

    public void InitializeBattlefield()
    {
        currentTurn = BattleObjectSide.Player;
        outcome = BattleOutcome.Neither;

        _map.Init();
        ArrangeCharacters(GetTestSquad(1), GetTestSquad(3));
    }

    // Расставляет юнитов на игровом поле
    public void ArrangeCharacters(IBattleCharacterInfo[] alliesInfo, IBattleCharacterInfo[] enemiesInfo)
    {
        BattleCharacter[] playerSquad = _characterFactory.CreatePlayerSquad(alliesInfo);
        for (int i = 0; i < playerSquad.Length; i++)
        {
            _characters.Add(playerSquad[i]);
            _map.SetCell(0, i, playerSquad[i]);
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
