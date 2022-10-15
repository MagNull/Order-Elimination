using System;
using System.Collections;
using System.Collections.Generic;
using UIManagement.Debugging;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class PauseMenuPanel : MonoBehaviour, IUIPanel, IDebuggablePanel<PauseMenuPanel>
    {
        private GameObject _gameObject;
        [SerializeField] private Button _saveGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;

        public event Action<IUIPanel> Opened;
        public event Action<IUIPanel> Closed;
        public event Action<PauseMenuPanel> SaveButtonPressed;
        public event Action<PauseMenuPanel> LoadButtonPressed;
        public event Action<PauseMenuPanel> ReturnButtonPressed;
        public event Action<PauseMenuPanel, float> MusicSliderValueChanged;
        public event Action<PauseMenuPanel, float> SoundSliderValueChanged;

        public string Title => "";
        public PanelType PanelType => PanelType.Pause;

        private float MusicSlderNormalizedValue
            => _musicSlider.value / (_musicSlider.maxValue - _musicSlider.minValue);

        private float SoundSlderNormalizedValue
            => _soundSlider.value / (_soundSlider.maxValue - _soundSlider.minValue);

        private void Awake()
        {
            _gameObject = gameObject;
            _saveGameButton.onClick.AddListener(OnSaveButtonPressed);
            _loadGameButton.onClick.AddListener(OnLoadButtonPressed);
            _returnButton.onClick.AddListener(OnReturnButtonPressed);
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);

            MusicSliderValueChanged += (panel, value) => MasterVolume.MusicVolume = value;
            SoundSliderValueChanged += (panel, value) => MasterVolume.SoundVolume = value;
        }

        public void Close()
        {
            _gameObject.SetActive(false);
            Closed?.Invoke(this);
        }

        public void Open()
        {
            _gameObject.SetActive(true);
            _musicSlider.value = MasterVolume.MusicVolume;
            _soundSlider.value = MasterVolume.SoundVolume;
            Opened?.Invoke(this);
        }

        public void OnMusicVolumeChanged(float value)
        {
            MusicSliderValueChanged?.Invoke(this, MusicSlderNormalizedValue);
        }

        public void OnSoundVolumeChanged(float value)
        {
            SoundSliderValueChanged?.Invoke(this, SoundSlderNormalizedValue);
        }

        private void OnSaveButtonPressed()
        {
            SaveButtonPressed?.Invoke(this);
        }

        private void OnLoadButtonPressed()
        {
            LoadButtonPressed?.Invoke(this);
        }

        private void OnReturnButtonPressed()
        {
            ReturnButtonPressed?.Invoke(this);
        }

        #region Debugging
        private Action<IUIPanel> openedDebug = (p) => Debug.Log($"{nameof(p.PanelType)}:{p.PanelType} \"{(p as PauseMenuPanel).name}\" opened");
        private Action<IUIPanel> closedDebug = (p) => Debug.Log($"{nameof(p.PanelType)}:{p.PanelType} \"{(p as PauseMenuPanel).name}\" closed");
        private Action<PauseMenuPanel> saveButtonDebug = (p) => p.ButtonPressedDebug(nameof(_saveGameButton));//Debug.Log($"{nameof(_saveGameButton)} pressed on {nameof(p.PanelType)}:{p.PanelType} \"{p.name}\"");
        private Action<PauseMenuPanel> loadButtonDebug = (p) => p.ButtonPressedDebug(nameof(_loadGameButton));
        private Action<PauseMenuPanel> returnButtonDebug = (p) => p.ButtonPressedDebug(nameof(_returnButton));
        private Action<PauseMenuPanel, float> musicSliderDebug 
            = (p, val) => Debug.Log($"{nameof(_musicSlider)}'s value changed on {nameof(p.PanelType)}:{p.PanelType} \"{p.name}\": {val}");
        private Action<PauseMenuPanel, float> soundSliderDebug
            = (p, val) => Debug.Log($"{nameof(_soundSlider)}'s value changed on {nameof(p.PanelType)}:{p.PanelType} \"{p.name}\": {val}");

        public void StartDebugging()
        {
            Opened += openedDebug;
            Closed += closedDebug;
            SaveButtonPressed += saveButtonDebug;
            LoadButtonPressed += loadButtonDebug;
            ReturnButtonPressed += returnButtonDebug;
            SoundSliderValueChanged += musicSliderDebug;
            MusicSliderValueChanged += soundSliderDebug;

        }

        public void StopDebugging()
        {
            Opened -= openedDebug;
            Closed -= closedDebug;
            SaveButtonPressed -= saveButtonDebug;
            LoadButtonPressed -= loadButtonDebug;
            ReturnButtonPressed -= returnButtonDebug;
            SoundSliderValueChanged -= musicSliderDebug;
            MusicSliderValueChanged -= soundSliderDebug;
        }
        #endregion Debugging
    }
}
