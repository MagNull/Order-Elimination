using System;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Start;
using Proyecto26;

namespace OrderElimination
{
    public class Database : MonoBehaviour
    {
        public static readonly string DatabaseLink = "https://orderelimination-default-rtdb.firebaseio.com/";
        private static readonly string _id = "b4e1c7e5-ff55-495e-b6c9-e63da38c2306";
        public static event Action<int, string> LoadSave;
        public static int SaveIndex { get; private set; }

        private void Awake()
        {
            SavesMenu.LoadClicked += SetMediatorBySelectedSave;
            SavesMenu.NewGameClicked += SetNewGame;
            BattleSimulation.BattleEnded += SetBattleOutcome;
        }

        private void Start()
        {
            PostSaveToDatabase(new List<Save> { new(), new(), new() });
            //RetrieveFromDatabase();
            LoadTextToSaves();
        }

        public static void PostSaveToDatabase(List<Save> saves)
        {
            var count = 0;
            foreach (var save in saves)
                RestClient.Put<List<Save>>(DatabaseLink + $"{_id}" + $"/{count++}" + ".json", save);
        }

        public static void PostSaveToDatabase(Save save, int saveIndex)
        {
            RestClient.Put<List<Save>>(DatabaseLink + $"{_id}" + $"/{saveIndex}" + ".json", save);
        }

        private void RetrieveSaveFromDatabase(int saveIndex)
        {
            RestClient.Get<List<Save>>(DatabaseLink + $"{_id}" + $"/{saveIndex}" + ".json")
                .Then(response =>
                {
                    // foreach (var save in response)
                    // {
                    //     Debug.Log(save.CountMove);
                    //     Debug.Log(save.SquadPositions[0]);
                    //     Debug.Log(save.EnemyPosition);
                    //     Debug.Log(save.IsMoveSquads[0]);
                    // }
                });
        }

        public static void DeleteSave(int index)
        {
            PlayerPrefs.DeleteKey($"{index}");
            PlayerPrefs.DeleteKey($"{SaveIndex}:Squad 0");
            PlayerPrefs.DeleteKey($"{SaveIndex}:Squad 1");
            PlayerPrefs.DeleteKey($"{SaveIndex}:Squad 0:isMove");
            PlayerPrefs.DeleteKey($"{SaveIndex}:Squad 1:isMove");
            PlayerPrefs.DeleteKey($"{SaveIndex}:CountMove");
            PlayerPrefs.DeleteKey($"{SaveIndex}:EnemySquad");
            PlayerPrefs.DeleteKey($"{SaveIndex}:BattleOutcome");
            PlayerPrefs.DeleteKey($"{SaveIndex}:Money");
        }

        public void LoadTextToSaves()
        {
            for (var i = 0; i < 3; i++)
            {
                if (!PlayerPrefs.HasKey($"{i}:CountMove"))
                    continue;
                var countMove = PlayerPrefs.GetInt($"{i}:CountMove");
                LoadSave?.Invoke(i, $"Игра {i + 1}, ход {countMove}");
            }
        }

        private void SetMediatorBySelectedSave(int saveIndex)
        {
            PlayerPrefs.SetInt($"SaveIndex", saveIndex);
            PlayerPrefs.SetString($"{SaveIndex}:BattleOutcome", BattleOutcome.Neither.ToString());
        }

        public void SetNewGame(int saveIndex)
        {
            var positions = new List<Vector3>
            {
                new (50, 150, 0),
                new (150, 110, 0)
            };
            SaveIndex = saveIndex;
            SetMediator(positions, new List<bool> { false, false }, 0, Vector3.zero, 0);
        }

        private void SetMediator(List<Vector3> positions, List<bool> isMoveSquads, int countMove, Vector3 enemyPosition,
            int money)
        {
            PlayerPrefs.SetInt($"SaveIndex", SaveIndex);
            PlayerPrefs.SetString($"{SaveIndex}:Squad 0", positions[0].ToString());
            PlayerPrefs.SetString($"{SaveIndex}:Squad 1", positions[1].ToString());
            PlayerPrefs.SetInt($"{SaveIndex}:Squad 0:isMove", isMoveSquads[0] ? 1 : 0);
            PlayerPrefs.SetInt($"{SaveIndex}:Squad 1:isMove", isMoveSquads[1] ? 1 : 0);
            PlayerPrefs.SetInt($"{SaveIndex}:CountMove", countMove);
            PlayerPrefs.SetString($"{SaveIndex}:EnemySquad", enemyPosition.ToString());
            PlayerPrefs.SetString($"{SaveIndex}:BattleOutcome", BattleOutcome.Neither.ToString());
            PlayerPrefs.SetInt($"{SaveIndex}:Money", money);
        }

        public static void SetBattleOutcome(BattleOutcome outcome)
        {
            PlayerPrefs.SetString($"{SaveIndex}:BattleOutcome", outcome.ToString());
        }

        private void OnDisable()
        {
            SavesMenu.LoadClicked -= SetMediatorBySelectedSave;
            SavesMenu.NewGameClicked -= SetNewGame;
        }
    }
}