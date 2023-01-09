using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Firebase.Database;
using OrderElimination.Start;
using UnityEngine.SceneManagement;

namespace OrderElimination
{
    public class Database : MonoBehaviour
    {
        private Dictionary<int, List<Vector3>> _allPositions;
        private Dictionary<int, int> _allCountMove;
        public static event Action<int, string> LoadSave;
        public static int SaveIndex { get; private set; }

        private void Awake()
        {
            Saves.LoadClicked += SetMediatorBySelectedSave;
            Saves.NewGameClicked += SetNewGame;
        }

        private void Start()
        {
            _allPositions = new Dictionary<int, List<Vector3>>();
            _allCountMove = new Dictionary<int, int>();
            LoadTextToSaves();
            LoadPositionsToSaves();
        }

        private static string GetIdFromFile()
        {
            using var reader = XmlReader
                .Create(Application.dataPath + "/Resources" + "/Xml" + "/Id.xml");
            reader.MoveToContent();
            return reader.ReadElementContentAsString();
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

        public static void SaveCountMove(int countMove)
        {
            FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .Child(SaveIndex.ToString())
                .Child("CountMove")
                .SetValueAsync(countMove);
        }

        public async void LoadTextToSaves()
        {
            var dataSnapshot = await FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .GetValueAsync();

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
            var dataSnapshot = await FirebaseDatabase
                .DefaultInstance
                .GetReference(GetIdFromFile())
                .Child("Saves")
                .GetValueAsync();
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
                
                _allPositions.Add(i, positions);
            }
        }

        private void SetMediatorBySelectedSave(int saveIndex)
        {
            if (SceneManager.GetActiveScene().name != "StartMenu")
                return;
            SaveIndex = saveIndex;
            StartMenuMediator.SetPositionsInSave(_allPositions[saveIndex]);
            StartMenuMediator.SetCountMove(_allCountMove[saveIndex]);
        }

        private void SetNewGame(int saveIndex)
        {
            var positions = new List<Vector3>
            {
                new Vector3(50, 150, 0),
                new Vector3(150, 110, 0)
            };
            SaveIndex = saveIndex;
            StartMenuMediator.SetPositionsInSave(positions);
            StartMenuMediator.SetCountMove(_allCountMove[saveIndex]);
            SaveData("Squad 0", new Vector3(50, 150, 0));
            SaveData("Squad 1", new Vector3(150, 110, 0));
            SaveCountMove(0);
        }
        
        private Vector3 GetVectorFromString(string str)
        {
            var temp = str.Split(new[] { "(", ")", ", ", ".00" }, StringSplitOptions.RemoveEmptyEntries);

            var x = Convert.ToInt32(temp[0]);
            var y = Convert.ToInt32(temp[1]);
            var z = Convert.ToInt32(temp[2]);
            return new Vector3(x, y, z);
        }
    }
}