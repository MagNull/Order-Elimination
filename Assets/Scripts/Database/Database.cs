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
        private static readonly int SaveCount = 3;
        
        private List<Save> _saves;
        public static event Action<int, string> LoadSave;
        public static int SaveIndex { get; private set; }

        private void Awake()
        {
            _saves = new List<Save>();
            SavesMenu.LoadClicked += SetMediatorBySelectedSave;
            SavesMenu.NewGameClicked += SetNewGame;
            BattleSimulation.BattleEnded += SetBattleOutcome;
        }

        private void Start()
        {
            RetrieveSaveFromDatabase();
        }

        public static void PutSaveToDatabase(List<Save> saves)
        {
            var count = 0;
            foreach (var save in saves)
                RestClient.Put<Save>(DatabaseLink + $"{_id}" + $"/{count++}" + ".json", save);
        }

        public static void PutSaveToDatabase(Save save, int saveIndex)
        {
            RestClient.Put<Save>(DatabaseLink + $"{_id}" + $"/{saveIndex}" + ".json", save);
        }

        private void RetrieveSaveFromDatabase()
        {
            _saves.Clear();
            RestClient
                .GetArray<Save>(DatabaseLink + $"{_id}"  + ".json")
                .Then(response =>
                {
                    _saves.AddRange(response);
                    LoadTextToSaves();
                });
        }

        public static void DeleteSave(int index)
        {
            RestClient.Delete(DatabaseLink + $"{_id}" + $"/{index}" + ".json");
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
            for (var i = 0; i < _saves.Count; i++)
                LoadSave?.Invoke(i, $"Игра {i + 1}, ход {_saves[i].CountMove}");
        }

        private void SetMediatorBySelectedSave(int saveIndex)
        {
            SaveIndex = saveIndex;
            SetMediator(_saves[saveIndex]);
        }

        public void SetNewGame(int saveIndex)
        {
            SaveIndex = saveIndex;
            PutSaveToDatabase(new Save(), saveIndex);
            SetMediator(new Save());
        }

        private void SetMediator(Save save)
        {
            PlayerPrefs.SetInt($"SaveIndex", SaveIndex);
            PlayerPrefs.SetString($"{SaveIndex}:Squad 0", save.SquadPositions[0].ToString());
            PlayerPrefs.SetString($"{SaveIndex}:Squad 1", save.SquadPositions[0].ToString());
            PlayerPrefs.SetInt($"{SaveIndex}:Squad 0:isMove", save.IsMoveSquads[0] ? 1 : 0);
            PlayerPrefs.SetInt($"{SaveIndex}:Squad 1:isMove", save.IsMoveSquads[1] ? 1 : 0);
            PlayerPrefs.SetInt($"{SaveIndex}:CountMove", save.CountMove);
            PlayerPrefs.SetString($"{SaveIndex}:EnemySquad", save.EnemyPosition.ToString());
            PlayerPrefs.SetString($"{SaveIndex}:BattleOutcome", BattleOutcome.Neither.ToString());
            PlayerPrefs.SetInt($"{SaveIndex}:Money", save.Money);
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