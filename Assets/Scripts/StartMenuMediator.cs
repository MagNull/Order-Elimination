using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Start
{
    public class StartMenuMediator : MonoBehaviour
    {
        private List<Vector3> _positionsInSave;
        private List<bool> _isMoveSquads;
        public int CountMoveInSave { get; private set; }
        public Vector3 EnemySquadPosition { get; private set; }
        public int SaveIndex { get; private set; }
        public int Money { get; private set; }
        public BattleOutcome Outcome { get; private set; } = BattleOutcome.Neither;
        public IReadOnlyList<bool> IsMoveSquads => _isMoveSquads;
        public IReadOnlyList<Vector3> PositionsInSave => _positionsInSave;
        public static StartMenuMediator Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Instance.EnemySquadPosition = Vector3.zero;
            BattleSimulation.BattleEnded += SetBattleOutcome;
        }
        
        public static void SetPositionsInSave(List<Vector3> positions)
        {
            Instance._positionsInSave = positions;
        }

        public static void SetIsMoveSquads(List<bool> isMoveSquads)
        {
            Instance._isMoveSquads = isMoveSquads;
            Database.SaveIsMoveSquads(isMoveSquads);
        }

        public static void SetIsMoveSquad(string squadName, bool isMove)
        {
            var squadNumber = Convert.ToInt32(squadName.Split().Last());
            Instance._isMoveSquads[squadNumber] = isMove;
            Database.SaveIsMoveSquads(Instance._isMoveSquads);
        }

        public static void SetEnemySquadPosition(Vector3 position)
        {
            Instance.EnemySquadPosition = position;
        }

        public static void SetCountMove(int countMove)
        {
            Instance.CountMoveInSave = countMove;
        }

        public static void SetMoney(int money)
        {
            Instance.Money = money;
            Database.SaveMoney(money);
        }

        public static void SetSaveIndex(int index)
        {
            Instance.SaveIndex = index;
        }

        public static void SetBattleOutcome(BattleOutcome outcome)
        {
            Instance.Outcome = outcome;
        }
    }
}