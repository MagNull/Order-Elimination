﻿using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.Start
{
    public class StartMenuMediator : MonoBehaviour
    {
        private List<Vector3> _positionsInSave;
        public int CountMoveInSave { get; private set; }
        public Vector3 EnemySquadPosition { get; private set; }
        public int SaveIndex { get; private set; }
        public int Money { get; private set; }
        public BattleOutcome Outcome { get; private set; } = BattleOutcome.Neither;
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