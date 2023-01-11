using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.Start;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OrderElimination
{
    public class InputClass : MonoBehaviour
    {
        [SerializeField] private SelectableObjects _selectableObjects;
        [SerializeField] private Database _database;
        [SerializeField] private Image _settingsImage;
        private ISelectable _selectedObject;
        public static event Action<Squad, PlanetPoint> TargetSelected;
        public static event Action onFinishMove;

        private void Awake() 
        { 
            Squad.Selected += ChangeSelectedObject;
            PlanetPoint.Onclick += PlanetPointClicked;
            StrategyMap.Onclick += ClickOnPanel;
        }

        private void Start()
        {
            Squad.onMove += SavePositions;
        }

        public void ClickOnPanel()
        {
            ChangeSelectedObject(null);
        }

        private void ChangeSelectedObject(ISelectable selectedObject)
        {
            if(_selectedObject is Squad squad)
            {
                squad.Unselect();
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
            if (end.CountSquadOnPoint == 2)
                return;
            TargetSelected?.Invoke(selectedSquad, end);
            selectedSquad.Unselect();
            selectedSquad.Move(end);
            
            StartMenuMediator.SetIsMoveSquad(selectedSquad.name, true);
        }

        private void SavePositions()
        {
            var squads = _selectableObjects.GetSquads();
            var count = 0;
            var positions = new List<Vector3>();
            foreach (var squad in squads)
            {
                if(squad.IsDestroyed())
                    continue;
                var position = squad.transform.position;
                positions.Add(position);
                _database.SaveData($"Squad {count++}", position);
            }
            
            StartMenuMediator.SetPositionsInSave(positions);
            StartMenuMediator.SetCountMove(StrategyMap.CountMove);
            Database.SaveCountMove(StrategyMap.CountMove);
            _database.LoadTextToSaves();
        }

        public void ResetDatabase()
        {
            _database.SetNewGame(StartMenuMediator.Instance.SaveIndex);
            SceneManager.LoadScene("StrategyMap");
        }

        public void FinishMove()
        {
            var squads = _selectableObjects.GetSquads();

            foreach (var squad in squads)
                squad.AlreadyMove = false;
            StrategyMap.AddCountMove();
            Database.SaveCountMove(StrategyMap.CountMove);
            _database.LoadTextToSaves();
            onFinishMove?.Invoke();
        }
        
        public void PauseButtonClicked()
        {
            _settingsImage.gameObject.SetActive(true);
        }
    }
}