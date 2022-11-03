using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SelectableObjects : MonoBehaviour
    {   
        private List<ISelectable> _canBeSelected;

        private void Awake() 
        {
            _canBeSelected = new List<ISelectable>();
            Creator.Created += AddSelectableObject;
        }

        private void AddSelectableObject(ISelectable selectableObject)
        {
            _canBeSelected.Add(selectableObject);
        }
    }
}