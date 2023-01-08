using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private Image _saves;
        [SerializeField] private Image _settings;
        private Button _playButton;
        private Button _settingsButton;
        private Button _exitButton;

        private void Awake()
        {
            _playButton = GameObject.Find("PlayButton").GetComponent<Button>();
            _settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
            _exitButton = GameObject.Find("ExitButton").GetComponent<Button>();

            Saves.ExitSavesWindow += SetActiveButtons;
            Settings.ExitSettingsWindow += SetActiveButtons;
        }

        private void Start()
        {
            _saves.gameObject.SetActive(false);
            _settings.gameObject.SetActive(false);
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
            _saves.gameObject.SetActive(true);
        }

        public void SettingsButtonClicked()
        {
            SetActiveButtons(false);
            _settings.gameObject.SetActive(true);
        }

        public void ExitButtonClicked()
        {
            Application.Quit();
        }
    }
}