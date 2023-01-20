using UnityEngine;
using System;
using System.Collections.Generic;
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
        public static event Action<bool> onPauseClicked;

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
            
            PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:{selectedSquad.name}:isMove", 1);
        }

        private void SavePositions()
        {
            if (this.IsDestroyed())
                return;
            var squads = _selectableObjects.GetSquads();
            var count = 0;
            var positions = new List<Vector3>();
            var isMoveSquads = new List<bool>();
            foreach (var squad in squads)
            {
                if(squad.IsDestroyed())
                    continue;
                var position = squad.transform.position;
                positions.Add(position);
                isMoveSquads.Add(squad.AlreadyMove);
                PlayerPrefs.SetString($"{StrategyMap.SaveIndex}:Squad {count}", position.ToString());
                //_database.SaveData($"Squad {count++}", position);
            }
            
            PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:CountMove", StrategyMap.CountMove);
            //Database.SaveCountMove(StrategyMap.CountMove);
            //Database.SaveIsMoveSquads(isMoveSquads);
            _database.LoadTextToSaves();
        }

        public void ResetDatabase()
        {
            _database.SetNewGame(StrategyMap.SaveIndex);
            SceneManager.LoadScene("StrategyMap");
        }

        public void FinishMove()
        {
            var squads = _selectableObjects.GetSquads();

            foreach (var squad in squads)
                squad.SetAlreadyMove(false);
            
            PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:Squad 0:isMove", 0);
            PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:Squad 1:isMove", 0);
            StrategyMap.AddCountMove();
            PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:CountMove", StrategyMap.CountMove);
            //Database.SaveCountMove(StrategyMap.CountMove);
            //Database.SaveIsMoveSquads(new List<bool>{false, false});
            _database.LoadTextToSaves();
            onFinishMove?.Invoke();
        }
        
        public void PauseButtonClicked()
        {
            onPauseClicked?.Invoke(false);
            _settingsImage.gameObject.SetActive(true);
        }

        public void OnDestroy()
        {
            Squad.onMove -= SavePositions;
        }
    }
}