using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class Saves : MonoBehaviour
    {
        [SerializeField] private Button _loadButton;
        [SerializeField] private List<Save> _saves;
        private int _selectedSaveIndex;
        private Image _selectedSavesImages;
        public static event Action<bool> ExitSavesWindow;
        public static event Action<int> LoadClicked;
        public static event Action<int> NewGameClicked;
        
        private void Awake()
        {
            Database.LoadSave += SetSaveText;
            foreach (var save in _saves)
                save.DeleteSave += DeleteSave;
            FirstSaveClicked();
            _loadButton.interactable = true;
        }

        public void FirstSaveClicked()
        {
            _selectedSaveIndex = 0;
            SwitchColorButtons();
        }

        public void SecondSaveClicked()
        {
            _selectedSaveIndex = 1;
            SwitchColorButtons();
        }

        public void ThirdSaveClicked()
        {
            _selectedSaveIndex = 2;
            SwitchColorButtons();
        }

        private void SwitchColorButtons()
        {
            for (var i = 0; i < _saves.Count; i++)
                _saves[i].SetActive(_selectedSaveIndex == i);
            
            _loadButton.interactable = !_saves[_selectedSaveIndex].IsEmpySave();
        }

        public void SetSaveText(int index, string text)
        {
            _saves[index].SetText(text);
        }

        public void LoadButtonClicked()
        {
            LoadClicked?.Invoke(_selectedSaveIndex);
            SceneManager.LoadScene("StrategyMap");
        }

        public void NewGameButtonClicked()
        {
            NewGameClicked?.Invoke(_selectedSaveIndex);
            SceneManager.LoadScene("StrategyMap");
        }

        public void ExitClicked()
        {
            ExitSavesWindow?.Invoke(true);
            gameObject.SetActive(false);
        }

        private void DeleteSave()
        {
            Database.DeleteSave(_selectedSaveIndex);
        }
    }
}