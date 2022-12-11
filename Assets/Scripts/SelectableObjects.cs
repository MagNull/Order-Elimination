using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SelectableObjects : MonoBehaviour
    {   
        private List<ISelectable> _selectableObjects;
        private void Awake() 
        {
            _selectableObjects = new List<ISelectable>();
            Creator.Created += AddSelectableObject;
        }

        private void AddSelectableObject(ISelectable selectableObject)
        {
            _selectableObjects.Add(selectableObject);
        }

        public List<Squad> GetSquads()
        {
            var squads = new List<Squad>();
            foreach (var temp in _selectableObjects)
                if(temp is Squad squad)
                    squads.Add(squad);
            return squads;
        }
    }
}