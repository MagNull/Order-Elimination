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
        public static event Action onSaveData;

        private void Awake() 
        { 
            Squad.Selected += ChangeSelectedObject;
            PlanetPoint.Onclick += PlanetPointClicked;
            StrategyMap.Onclick += ClickOnPanel;
        }

        private void Start()
        {
            Squad.onMove += SaveData;
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

        private void SaveData()
        {
            onSaveData?.Invoke();
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
            
            SaveData();
            onFinishMove?.Invoke();
        }
        
        public void PauseButtonClicked()
        {
            onPauseClicked?.Invoke(false);
            _settingsImage.gameObject.SetActive(true);
        }

        public void OnDestroy()
        {
            Squad.onMove -= SaveData;
        }
    }
}