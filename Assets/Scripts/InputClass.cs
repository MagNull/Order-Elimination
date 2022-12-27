using UnityEngine;
using System;
using System.Xml;
using Unity.VisualScripting;

namespace OrderElimination
{
    public class InputClass : MonoBehaviour
    {
        [SerializeField] private SelectableObjects _selectableObjects;
        [SerializeField] private Database _database;
        private ISelectable _selectedObject;
        public static event Action<Squad, PlanetPoint> TargetSelected;
    
        private void Awake() 
        { 
            Squad.Selected += ChangeSelectedObject;
            PlanetPoint.Onclick += PlanetPointClicked;
            StrategyMap.Onclick += ClickOnPanel;
        }

        public void ClickOnPanel()
        {
            ChangeSelectedObject(null);
        }

        private void ChangeSelectedObject(ISelectable selectedObject)
        {
            if(_selectedObject is Squad)
            {
                ((Squad)_selectedObject).Unselect();
            }
            _selectedObject = selectedObject;
        }

        private void PlanetPointClicked(PlanetPoint selectedPoint)
        {
            if(_selectedObject is not Squad)
            {
                ChangeSelectedObject(selectedPoint);
                return;
            }

            var selectedSquad = (Squad)_selectedObject;
            if(selectedSquad is null || selectedPoint is null)
                return;
            
            foreach(var end in selectedSquad.PlanetPoint.GetNextPoints())
                if(end == selectedPoint)
                    TargetIsClicked(selectedSquad, selectedPoint);
            
            ChangeSelectedObject(null);
        }

        private void TargetIsClicked(Squad selectedSquad, PlanetPoint end)
        {
            TargetSelected?.Invoke(selectedSquad, end);
            selectedSquad.Unselect();
            selectedSquad.Move(end);
            SavePositionToXml();
        }

        private void SavePositionToXml()
        {
            var squads = _selectableObjects.GetSquads();
            var count = 0;
            
            foreach (var squad in squads)
            {
                if(squad.IsDestroyed())
                    continue;
                var position = squad.transform.position;
                _database.SaveData($"Squad {count++}", position);
            }
        }

        public void FinishMove()
        {
            var squads = _selectableObjects.GetSquads();

            foreach (var squad in squads)
                squad.AlreadyMove = false;
        }
    }
}