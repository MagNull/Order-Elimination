using RoguelikeMap.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace StartSessionMenu
{
    public class StartMusicController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource.Play();
            SettingsPanel.OnMusicVolumeChanged += ChangeVolume;
        }

        private void ChangeVolume(int volume)
        {
            _audioSource.volume = volume / 100f;
        }

        void OnDestroy()
        {
            SettingsPanel.OnMusicVolumeChanged -= ChangeVolume;
        }
    }
}