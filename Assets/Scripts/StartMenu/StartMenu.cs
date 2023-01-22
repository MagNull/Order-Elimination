using System;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private SavesMenu savesMenu;
        [SerializeField] private GameObject loginForm;
        private Button _playButton;
        private Button _settingsButton;
        private Button _exitButton;

        public static event Action OnPlayerLogin;

        private void Awake()
        {
            _playButton = GameObject.Find("PlayButton").GetComponent<Button>();
            _exitButton = GameObject.Find("ExitButton").GetComponent<Button>();

            SavesMenu.ExitSavesWindow += SetActiveButtons;
            Settings.ExitSettingsWindow += SetActiveButtons;
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey("Id"))
            {
                loginForm.SetActive(true);
            }
            OnPlayerLogin?.Invoke();
        }

        private void SetActiveButtons(bool isActive)
        {
            if(_playButton != null)
                _playButton.interactable = isActive;
            if(_settingsButton != null)
                _settingsButton.interactable = isActive;
            if(_exitButton != null)
                _exitButton.interactable = isActive;
        }
        
        public void StartButtonClicked()
        {
            SetActiveButtons(false);
            savesMenu.SetActive(true);
        }

        public void SettingsButtonClicked()
        {
            SetActiveButtons(false);
        }

        public void ExitButtonClicked()
        {
            Application.Quit();
        }
    }
}