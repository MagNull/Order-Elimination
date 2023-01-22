using System;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Start;
using Proyecto26;

namespace OrderElimination
{
    public class Database : MonoBehaviour
    {
        public static readonly string[] SquadNames = { "Squad 0", "Squad 1" };
        private static readonly string DatabaseLink = "https://orderelimination-default-rtdb.firebaseio.com/users";
        private static readonly string _id = "b4e1c7e5-ff55-495e-b6c9-e63da38c2306";
        private static readonly int SaveCount = 3;
        
        private List<Save> _saves;
        public static event Action<int, string> LoadSave;

        private void Awake()
        {
            // _saves = new List<Save>();
            // SavesMenu.LoadClicked += SetMediatorBySelectedSave;
            // SavesMenu.NewGameClicked += SetNewGame;
            // BattleSimulation.BattleEnded += SetBattleOutcome;
        }

        private void Start()
        {
            //RetrieveSaveFromDatabase();
        }

        public static void SendToDatabase(UserData userData, string separator)
        {
            RestClient.Put<UserData>(DatabaseLink + "/" + separator + ".json", userData);
        }

        public static void GetUserByLogin(string login,
            Action<RequestException, ResponseHelper, UserData> getInfoCallback)
        {
            RestClient.Get<UserData>($"{DatabaseLink}/{login}.json", getInfoCallback);
        }

        public static void FindUserByEmail(string email, Action<RequestException, ResponseHelper> getInfoCallback)
        {
            RestClient.Get($"{DatabaseLink}.json?orderBy=%22Email%22&equalTo=%22{email}%22", getInfoCallback);
        }

        public static void PutSaveToDatabase(List<Save> saves)
        {
            var count = 0;
            foreach (var save in saves)
            {
                PutSaveToDatabase(save, count);
                count++;
            }
        }

        public static void PutSaveToDatabase(Save save, int saveIndex)
        {
            RestClient.Put<Save>(DatabaseLink + $"{_id}" + $"/{saveIndex}" + ".json", save);
            SetPlayerPrefs(save, saveIndex);
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

        public static void DeleteEnemyPosition(int index)
        {
            RestClient.Delete(DatabaseLink + $"{_id}" + $"/{index}" + "/EnemyPosition" + ".json");
            PlayerPrefs.DeleteKey($"{index}:CountMove");
        }

        public static void DeleteSave(int index)
        {
            RestClient.Delete(DatabaseLink + $"{_id}" + $"/{index}" + ".json");
            PlayerPrefs.DeleteKey($"{index}");
            foreach (var squadName in SquadNames)
            {
                PlayerPrefs.DeleteKey($"{index}:{squadName}");
                PlayerPrefs.DeleteKey($"{index}:{squadName}:isMove");
            }
            
            PlayerPrefs.DeleteKey($"{index}:CountMove");
            PlayerPrefs.DeleteKey($"{index}:EnemySquad");
            PlayerPrefs.DeleteKey($"{index}:BattleOutcome");
            PlayerPrefs.DeleteKey($"{index}:Money");
        }

        public void LoadTextToSaves()
        {
            for (var i = 0; i < _saves.Count; i++)
                LoadSave?.Invoke(i, $"Игра {i + 1}, ход {_saves[i].CountMove}");
        }

        private void SetMediatorBySelectedSave(int saveIndex)
        {
            SetPlayerPrefs(_saves[saveIndex], saveIndex);
        }

        public void SetNewGame(int saveIndex)
        {
            PutSaveToDatabase(new Save(), saveIndex);
            SetPlayerPrefs(new Save(), saveIndex);
        }

        private static void SetPlayerPrefs(Save save, int saveIndex)
        {
            PlayerPrefs.SetInt($"SaveIndex", saveIndex);
            var count = 0;
            foreach (var squadName in SquadNames)
            {
                PlayerPrefs.SetString($"{saveIndex}:{squadName}", save.SquadPositions[count].ToString());
                PlayerPrefs.SetInt($"{saveIndex}:{squadName}:isMove", save.IsMoveSquads[count++] ? 1 : 0);
            }
            
            PlayerPrefs.SetInt($"{saveIndex}:CountMove", save.CountMove);
            PlayerPrefs.SetString($"{saveIndex}:EnemySquad", save.EnemyPosition.ToString());
            PlayerPrefs.SetString($"{saveIndex}:BattleOutcome", BattleOutcome.Neither.ToString());
            PlayerPrefs.SetInt($"{saveIndex}:Money", save.Money);
        }

        private static void SetBattleOutcome(BattleOutcome outcome)
        {
            PlayerPrefs.SetString($"{StrategyMap.SaveIndex}:BattleOutcome", outcome.ToString());
        }

        private void OnDisable()
        {
            SavesMenu.LoadClicked -= SetMediatorBySelectedSave;
            SavesMenu.NewGameClicked -= SetNewGame;
        }
    }
}