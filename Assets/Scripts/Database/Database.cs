using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Firebase.Database;

namespace OrderElimination
{
    public class Database : MonoBehaviour
    {
        private DatabaseReference _dbReference;
        private List<Vector3> _positions;
        [SerializeField]
        private string _id;

        private void Awake()
        {
            _dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        private string GetIdFromFile()
        {
            using var reader = XmlReader
                .Create(Application.dataPath + "/Resources" + "/Xml" + "/Id.xml");
            reader.MoveToContent();
            return reader.ReadElementContentAsString();
        }

        public void SaveData(string squadName, Vector3 position)
        {
            _dbReference
                .Child("Positions")
                .Child(_id)
                .Child(squadName)
                .SetValueAsync(position.ToString());
        }

        public async IAsyncEnumerable<string> LoadData()
        {
            var dataSnapshot = await FirebaseDatabase
                .DefaultInstance
                .GetReference("Positions")
                .Child(_id)
                .GetValueAsync();

            if (dataSnapshot is null)
                throw new NullReferenceException();
            
            foreach (var child in dataSnapshot.Children)
            {
                var positionString = child.Value.ToString();
                yield return positionString;
            }
        }
    }
}