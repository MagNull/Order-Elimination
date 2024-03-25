using System.Collections.Generic;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.UI;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace RoguelikeMap
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _musicSource;

        [SerializeField]
        private AudioSource _soundSource;

        [SerializeField]
        private List<AudioResource> _soundsList = new();

        private int _activeMusicIndex = -1;

        [ShowInInspector]
        private const int FirstMapSoundIndex = 0;
        [ShowInInspector]
        private const int SecondMapSoundIndex = 1;
        [ShowInInspector]
        private const int SafeEventSoundIndex = 2;
        [ShowInInspector]
        private const int BattleEventSoundIndex = 3;
        [ShowInInspector]
        private const int SafeZoneSoundIndex = 4;
        [ShowInInspector]
        private const int ShopSoundIndex = 5;

        [Inject]
        public void Construct(PanelManager panelManager)
        {
            SubscribeToPanelEvents(panelManager);
        }

        private void Start()
        {
            SettingsPanel.OnMusicVolumeChanged += ChangeMusicVolume;
            SettingsPanel.OnSoundVolumeChanged += ChangeSoundVolume;
            PlayMapSound();
        }

        private void OnDestroy()
        {
            SettingsPanel.OnMusicVolumeChanged -= ChangeMusicVolume;
            SettingsPanel.OnSoundVolumeChanged -= ChangeSoundVolume;
        }

        private void SubscribeToPanelEvents(PanelManager panelManager)
        {
            var eventPanel = panelManager.GetPanelByPointInfo(PointType.Event) as EventPanel;
            eventPanel.OnBattleEventVisit += PlayBattleEventSound;
            eventPanel.OnSafeEventVisit += PlaySafeEventSound;
            eventPanel.OnPlaySound += PlaySound;

            var safeZonePanel = panelManager.GetPanelByPointInfo(PointType.SafeZone) as SafeZonePanel;
            safeZonePanel.OnSafeZoneVisit += PlaySafeZoneSound;

            var shopPanel = panelManager.GetPanelByPointInfo(PointType.Shop) as ShopPanel;
            shopPanel.OnShopVisit += PlayShopSound;
        }

        private void PlayMapSound() => SetActiveMusic(GetRandomMapSound());
        private void PlaySafeEventSound(bool isPlay) => SetActiveMusic(isPlay ? SafeEventSoundIndex : GetRandomMapSound());
        private void PlayBattleEventSound(bool isPlay) => SetActiveMusic(isPlay ? BattleEventSoundIndex : GetRandomMapSound());
        private void PlaySafeZoneSound(bool isPlay) => SetActiveMusic(isPlay ? SafeZoneSoundIndex : GetRandomMapSound());
        private void PlayShopSound(bool isPlay) => SetActiveMusic(isPlay ? ShopSoundIndex : GetRandomMapSound());

        private int GetRandomMapSound()
        {
            return Random.Range(0, 2);
        }

        private void SetActiveMusic(int index)
        {
            if (index < 0 || index >= _soundsList.Count) return;

            _activeMusicIndex = index;
            PlayMusic(_soundsList[_activeMusicIndex]);
        }

        private void PlayMusic(AudioResource music)
        {
            _musicSource.resource = music;
            _musicSource.Play();
        }

        private void PlaySound(AudioResource sound)
        {
            _soundSource.resource = sound;
            _soundSource.Play();
        }

        private void ChangeMusicVolume() => ChangeMusicVolume(PlayerPrefs.GetInt(SettingsPanel.MusicVolumeKey, 100));

        private void ChangeMusicVolume(int volume)
        {
            _musicSource.volume = volume / 100f;
        }

        private void ChangeSoundVolume(int volume)
        {
            _soundSource.volume = volume / 100f;
        }
    }
}