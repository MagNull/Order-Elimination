using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class SelectedObject : MonoBehaviour
    {   
        private List<ISelectable> _canBeSelected;

        private void Awake() 
        {
            Creator.Created += AddToList;
        }

        private void AddToList(ISelectable selectableObject)
        {
            _canBeSelected.Add(selectableObject);
        }
    }

    
}