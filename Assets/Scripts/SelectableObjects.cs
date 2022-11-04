using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SelectableObjects : MonoBehaviour
    {   
        private List<ISelectable> _canBeSelected;
        static public List<ISelectable> _selectedObjects;
        private void Awake() 
        {
            _canBeSelected = new List<ISelectable>();
            _selectedObjects = new List<ISelectable>();
            Creator.Created += AddSelectableObject;
        }

        private void AddSelectableObject(ISelectable selectableObject)
        {
            _canBeSelected.Add(selectableObject);
        }
    }
}