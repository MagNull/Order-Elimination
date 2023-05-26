using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using Unity.Services.Core;
using UnityEngine;
using VContainer;

namespace RoguelikeMap
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField]
        private List<AudioSource> musicList = new List<AudioSource>();
        private int activeMusicIndex = -1;
        private PanelGenerator _panelGenerator;
        
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

        [Inject]
        public void Construct(PanelGenerator panelGenerator)
        {
            _panelGenerator = panelGenerator;
            panelGenerator.OnInitializedPanels += SubscribeToPanelEvents;
        }

        private void Start()
        {
            PlayMapSound();
        }

        private void SubscribeToPanelEvents()
        {
            var eventPanel = _panelGenerator.GetPanelByPointInfo(PointType.Event) as EventPanel;
            eventPanel.OnBattleEventVisit += PlayBattleEventSound;
            eventPanel.OnSafeEventVisit += PlaySafeEventSound;

            var safeZonePanel = _panelGenerator.GetPanelByPointInfo(PointType.SafeZone) as SafeZonePanel;
            safeZonePanel.OnSafeZoneVisit += PlaySafeZoneSound;
        }

        private void PlayMapSound() => SetActiveMusic(GetRandomMapSound());
        private void PlaySafeEventSound(bool isPlay) => SetActiveMusic(isPlay ? SafeEventSoundIndex : GetRandomMapSound());
        private void PlayBattleEventSound(bool isPlay) => SetActiveMusic(isPlay ? BattleEventSoundIndex : GetRandomMapSound());
        private void PlaySafeZoneSound(bool isPlay) => SetActiveMusic(isPlay ? SafeZoneSoundIndex: GetRandomMapSound());

        private int GetRandomMapSound()
        {
            return Random.Range(0, 2);
        }
        
        private void SetActiveMusic(int index)
        {
            foreach (var audioSource in musicList.Where(audioSource => audioSource != musicList[index]))
            {
                audioSource.Stop();
            }

            if (index < 0 || index >= musicList.Count) return;
            
            activeMusicIndex = index;
            musicList[activeMusicIndex].Play();
        }
    }
}