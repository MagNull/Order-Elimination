using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class SavesMenu : MonoBehaviour
    {
        [SerializeField] private Button _loadButton;
        [SerializeField] private List<SaveButton> _saves;
        [SerializeField] private Image _blackout;
        private int _selectedSaveIndex;
        private Image _selectedSavesImages;
        public float DURATION = 0.6f;
        public float END_VALUE = 0.8f;
        public static event Action<bool> ExitSavesWindow;
        public static event Action<int> LoadClicked;
        public static event Action<int> NewGameClicked;
        
        private void Awake()
        {
            Database.LoadSave += SetSaveText;
            AuthManager.OnUserLogout += ClearSaveText;
            foreach (var save in _saves)
                save.DeleteSave += DeleteSave;
            SaveClicked(0);
            _loadButton.interactable = true;
            
            SetActive(false);
            _blackout.gameObject.SetActive(false);
        }
        

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void SaveClicked(int index)
        {
            _selectedSaveIndex = index;
            SwitchColorButtons();
        }

        private void SwitchColorButtons()
        {
            for (var i = 0; i < _saves.Count; i++)
                _saves[i].SetActive(_selectedSaveIndex == i);
            
            _loadButton.interactable = !_saves[_selectedSaveIndex].IsEmptySave();
        }

        public void SetSaveText(int index, string text)
        {
            _saves[index].SetText(text);
        }

        public void ClearSaveText()
        {
            foreach (var save in _saves)
                save.SetText("");
        }

        public async void LoadButtonClicked()
        {
            _blackout.gameObject.SetActive(true);
            await _blackout.DOFade(END_VALUE, DURATION).AsyncWaitForCompletion();
            LoadClicked?.Invoke(_selectedSaveIndex);
            SceneManager.LoadScene(1);
        }

        public async void NewGameButtonClicked()
        {
            _blackout.gameObject.SetActive(true);
            await _blackout.DOFade(END_VALUE, DURATION).AsyncWaitForCompletion();
            NewGameClicked?.Invoke(_selectedSaveIndex);
            SceneManager.LoadScene(1);
        }

        public void ExitClicked()
        {
            ExitSavesWindow?.Invoke(true);
            gameObject.SetActive(false);
        }

        private void DeleteSave()
        {
            Database.DeleteSave(_selectedSaveIndex);
            _loadButton.interactable = false;
        }
    }
}