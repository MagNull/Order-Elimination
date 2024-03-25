using System;
using Unity.VisualScripting;
using UnityEngine;

namespace RoguelikeMap.UI
{
    public class SettingsPanel : Panel
    {
        [SerializeField]
        VolumeSlider _musicSlider;

        [SerializeField]
        VolumeSlider _soundSlider;

        public static string MusicVolumeKey = "MusicVolume";
        public static string SoundVolumeKey = "SoundVolume";

        public static event Action<int> OnMusicVolumeChanged;
        public static event Action<int> OnSoundVolumeChanged;

        public void Start()
        {
            _musicSlider.OnVolumeChanged += MusicVolumeChanged;
            _soundSlider.OnVolumeChanged += SoundVolumeChanged;
            if (!PlayerPrefs.HasKey(MusicVolumeKey))
                PlayerPrefs.SetInt(MusicVolumeKey, 100);

            if (!PlayerPrefs.HasKey(SoundVolumeKey))
                PlayerPrefs.SetInt(SoundVolumeKey, 100);

            MusicVolumeChanged(PlayerPrefs.GetInt(MusicVolumeKey));
            SoundVolumeChanged(PlayerPrefs.GetInt(SoundVolumeKey));
            CloseWithoutAnimation();
        }

        public override void Open()
        {
            var musicVolume = PlayerPrefs.HasKey(MusicVolumeKey) ? PlayerPrefs.GetInt(MusicVolumeKey) : 100;
            _musicSlider.Initialize(musicVolume);
            OnMusicVolumeChanged?.Invoke(musicVolume);

            var soundVolume = PlayerPrefs.HasKey(SoundVolumeKey) ? PlayerPrefs.GetInt(SoundVolumeKey) : 100;
            _soundSlider.Initialize(soundVolume);
            OnSoundVolumeChanged?.Invoke(soundVolume);

            base.Open();
        }

        private void MusicVolumeChanged(int volume)
        {
            PlayerPrefs.SetInt(MusicVolumeKey, volume);
            OnMusicVolumeChanged?.Invoke(volume);
        }

        private void SoundVolumeChanged(int volume)
        {
            PlayerPrefs.SetInt(SoundVolumeKey, volume);
            OnSoundVolumeChanged?.Invoke(volume);
        }
    }
}