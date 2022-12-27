using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Firebase.Database;

namespace OrderElimination
{
    public class Database : MonoBehaviour
    {
        private DatabaseReference _dbReference;
        private List<Vector3> _positions;

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
                .Child(GetIdFromFile())
                .Child(squadName)
                .SetValueAsync(position.ToString());
        }

        public async Task LoadData()
        {
            var dataSnapshot = await FirebaseDatabase
                .DefaultInstance
                .GetReference("Positions")
                .Child(GetIdFromFile())
                .GetValueAsync();

            if (dataSnapshot is null)
                throw new NullReferenceException();

            using var writer =
                XmlWriter.Create(Application.dataPath + "/Resources" + "/Xml" + "/SquadPositions.xml");

            writer.WriteStartElement("Positions");
            foreach (var child in dataSnapshot.Children)
            {
                var positionString = child.Value.ToString();
                writer.WriteRaw(positionString);
            }

            writer.WriteEndElement();
        }
    }
}