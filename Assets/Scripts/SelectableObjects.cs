using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SelectableObjects : MonoBehaviour
    {   
        private Dictionary<ISelectable, bool> _canBeSelected;
        private void Awake() 
        {
            _canBeSelected = new Dictionary<ISelectable, bool>();
            Creator.Created += AddSelectableObject;
        }

        private void AddSelectableObject(ISelectable selectableObject)
        {
            _canBeSelected.Add(selectableObject, false);
        }
    }
}