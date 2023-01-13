using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Firebase.Database;
using OrderElimination.Start;
using UnityEngine.SceneManagement;

namespace OrderElimination
{
    public class Database : MonoBehaviour
    {
        private static string _id = "b4e1c7e5-ff55-495e-b6c9-e63da38c2306";
        
        private Dictionary<int, List<Vector3>> _allPositions;
        private Dictionary<int, Vector3> _enemySquadPositions;
        private Dictionary<int, List<bool>> _isMovesSquadsOnSaves;
        private Dictionary<int, int> _moneyInSaves;
        private Dictionary<int, int> _allCountMove;
        public static event Action<int, string> LoadSave;
        public static int SaveIndex { get; private set; }

        private void Awake()
        {
            Debug.Log("Awake DB");
            Saves.LoadClicked += SetMediatorBySelectedSave;
            Saves.NewGameClicked += SetNewGame;
        }

        private void Start()
        {
            _allPositions = new Dictionary<int, List<Vector3>>();
            _isMovesSquadsOnSaves = new Dictionary<int, List<bool>>();
            _allCountMove = new Dictionary<int, int>();
            _moneyInSaves = new Dictionary<int, int>();
            _enemySquadPositions = new Dictionary<int, Vector3>();
            LoadTextToSaves();
            LoadPositionsToSaves();
            LoadEnemySquadPositionToSaves();
            LoadMoney();
            LoadIsMoveSquads();
        }

        private static string GetIdFromFile()
        {
            return _id;
        }

        public void SaveData(string squadName, Vector3 position)
        {
            var dataSnapshot = FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .Child(SaveIndex.ToString());
            dataSnapshot
                .Child("Positions")
                .Child(squadName)
                .SetValueAsync(position.ToString());
            var dateTime = DateTime.Now;
            dataSnapshot
                .Child("Time")
                .SetValueAsync($"{dateTime.ToShortTimeString()} - {dateTime.ToShortDateString()}");
        }

        public static void DeleteSave(int index)
        {
            FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .Child(index.ToString())
                .RemoveValueAsync();
        }

        public static void SaveCountMove(int countMove, string child = "CountMove")
        {
            SaveChild(child, countMove.ToString());
        }

        public static void SaveEnemySquadPosition(Vector3 position, string child = "EnemyPosition")
        {
            SaveChild(child, position.ToString());
        }

        public static void SaveIsMoveSquads(List<bool> isMoveSquads)
        {
            foreach(var isMoveSquad in isMoveSquads)
                SaveChild("Squad 0", isMoveSquad.ToString());
        }

        public static void SaveMoney(int money, string child = "Money")
        {
            SaveChild(child, money.ToString());
        }

        public static void SaveChild(string child, string value)
        {
            FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .Child(SaveIndex.ToString())
                .Child(child)
                .SetValueAsync(value);
        }
        
        public static void DeleteEnemySquadPosition(string child = "EnemyPosition")
        {
            DeleteChild(child);
        }

        public static void DeleteChild(string child)
        {
            FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .Child(SaveIndex.ToString())
                .Child(child)
                .RemoveValueAsync();
        }

        public async void LoadTextToSaves()
        {
            var dataSnapshot = await GetSavesDataSnapshot();

            for (var i = 0; i < 3; i++)
            {
                if(!dataSnapshot.Child($"{i}").Child("CountMove").Exists)
                    continue;
                var countMove = Convert.ToInt32(dataSnapshot.Child($"{i}").Child("CountMove").Value.ToString());
                if (countMove == -1)
                    continue;
                var timeString = dataSnapshot.Child($"{i}").Child("Time").Value.ToString();
                LoadSave?.Invoke(i, $"Игра {i + 1}, ход {countMove} ({timeString})");
                _allCountMove[i] = countMove;
            }
        }

        private async void LoadPositionsToSaves()
        {
            var dataSnapshot = await GetSavesDataSnapshot();
            for (var i = 0; i < dataSnapshot.ChildrenCount; i++)
            {
                var positionsSnapshot = dataSnapshot.Child($"{i}").Child("Positions");
                if (!positionsSnapshot.Child("Squad 0").Exists)
                    return;
                var firstSquadPositionString = positionsSnapshot.Child("Squad 0").Value.ToString();
                var secondSquadPositionString = positionsSnapshot.Child("Squad 1").Value.ToString();
                
                var positions = new List<Vector3>
                {
                    GetVectorFromString(firstSquadPositionString),
                    GetVectorFromString(secondSquadPositionString)
                };
                
                _allPositions[i] = positions;
            }
        }

        private async void LoadIsMoveSquads()
        {
            var dataSnapshot = await GetSavesDataSnapshot();

            for (var i = 0; i < dataSnapshot.ChildrenCount; i++)
            {
                var isMoveSnapshot = dataSnapshot.Child($"{i}").Child("IsMove");
                var firstSquadIsMove = "0";
                var secondSquadIsMove = "0";
                
                if (isMoveSnapshot.Child("Squad 0").Exists)
                    firstSquadIsMove = isMoveSnapshot.Child("Squad 0").Value.ToString();
                if (isMoveSnapshot.Child("Squad 1").Exists)
                    secondSquadIsMove = isMoveSnapshot.Child("Squad 1").Value.ToString();
                
                var isMoveSquads = new List<bool>
                {
                    firstSquadIsMove != "0",
                    secondSquadIsMove != "0"
                };
                
                _isMovesSquadsOnSaves[i] = isMoveSquads;
            }
        }

        private async void LoadMoney()
        {
            var dataSnapshot = await GetSavesDataSnapshot();
            
            for (var i = 0; i < dataSnapshot.ChildrenCount; i++)
            {
                var money = dataSnapshot.Child($"{i}").Child("Money").Exists
                    ? dataSnapshot.Child($"{i}").Child("Money").Value.ToString()
                    : "0";
                _moneyInSaves[i] = Convert.ToInt32(money);
            }
        }

        private async void LoadEnemySquadPositionToSaves()
        {
            var dataSnapshot = await GetSavesDataSnapshot();
            
            for (var i = 0; i < dataSnapshot.ChildrenCount; i++)
            {
                if (!dataSnapshot.Child($"{i}").Child("EnemyPosition").Exists)
                {
                    _enemySquadPositions[i] = Vector3.zero;    
                    continue;
                }
                    
                var positionString = dataSnapshot.Child($"{i}").Child("EnemyPosition").Value.ToString();
                _enemySquadPositions[i] = GetVectorFromString(positionString);
            }
        }

        private async Task<DataSnapshot> GetSavesDataSnapshot()
        {
            return await FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .GetValueAsync();
        }

        private void SetMediatorBySelectedSave(int saveIndex)
        {
            if (SceneManager.GetActiveScene().name != "StartMenu")
                return;
            SaveIndex = saveIndex;
            SetMediator(_allPositions[saveIndex], _isMovesSquadsOnSaves[saveIndex], _allCountMove[saveIndex],
                _enemySquadPositions[saveIndex], _moneyInSaves[saveIndex]);
        }

        public void SetNewGame(int saveIndex)
        {
            var positions = new List<Vector3>
            {
                new Vector3(50, 150, 0),
                new Vector3(150, 110, 0)
            };
            SaveIndex = saveIndex;
            SetMediator(positions, new List<bool>{false, false} ,0, Vector3.zero, 0);
            SaveData("Squad 0", new Vector3(50, 150, 0));
            SaveData("Squad 1", new Vector3(150, 110, 0));
            SaveCountMove(0);
        }

        private void SetMediator(List<Vector3> positions,List<bool> isMoveSquads, int countMove, Vector3 enemyPosition, int money)
        {
            StartMenuMediator.SetSaveIndex(SaveIndex);
            StartMenuMediator.SetPositionsInSave(positions);
            StartMenuMediator.SetIsMoveSquads(isMoveSquads);
            StartMenuMediator.SetCountMove(countMove);
            StartMenuMediator.SetEnemySquadPosition(enemyPosition);
            StartMenuMediator.SetMoney(money);
        }
        
        private Vector3 GetVectorFromString(string str)
        {
            var temp = str.Split(new[] { "(", ")", ", ", ".00" }, StringSplitOptions.RemoveEmptyEntries);

            var x = Convert.ToInt32(temp[0]);
            var y = Convert.ToInt32(temp[1]);
            var z = Convert.ToInt32(temp[2]);
            return new Vector3(x, y, z);
        }

        private void OnDisable()
        {
            Saves.LoadClicked -= SetMediatorBySelectedSave;
            Saves.NewGameClicked -= SetNewGame;
        }
    }
}