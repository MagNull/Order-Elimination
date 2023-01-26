using System;
using System.Linq;
using System.Threading.Tasks;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using OrderElimination;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tutorial
{
    public class StrategyTutorial : SerializedMonoBehaviour
    {
        [SerializeField]
        private StrategyTutorialStage[] _tutorialStages;
        [SerializeField]
        private Unmask[] _unmasks;
        [SerializeField]
        private StrategyMap _strategyMap;
        [SerializeField]
        private TextMeshProUGUI _tutorialText;
        private int _activeCutoutCounter = 0;

        [Button]
        private void ResetTutorial()
        {
            PlayerPrefs.SetInt("Strategy Tutorial", 1);
        }
        
        private async void Start()
        {
            if (PlayerPrefs.GetInt("Strategy Tutorial") < 0)
            {
                Destroy(gameObject);
                return;
            }

            await StartTutorial();
        }

        private async Task StartTutorial()
        {
            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            gameObject.SetActive(false);
            await UniTask.Delay(300);
            _strategyMap.SpawnEnemy(3);
            foreach (var stage in _tutorialStages)
            {
                await ProceedStage(stage);
            }
            PlayerPrefs.SetInt("Strategy Tutorial", -1);
        }

        private async Task ProceedStage(StrategyTutorialStage stage)
        {
            gameObject.SetActive(true);
            bool stateBefore = true;
            if (!stage.NeedMoveToPlanet && stage.ClickTargets.Length > 0)
            {
                stateBefore = stage.ClickTargets[0].gameObject.activeSelf;
                stage.ClickTargets[0].gameObject.SetActive(true);
            }
            _tutorialText.text = stage.Text;
            stage.AppearingObject.SetActive(true);


            if (stage.UnmaskEnemy)
            {
                HighlightObject(_strategyMap.EnemySquad.gameObject);
            }

            foreach (var obj in stage.UnmaskObjects)
            {
                HighlightObject(obj.gameObject);
            }

            foreach (var pointIndex in stage.UnmaskPlanetPointIndexes)
            {
                var planetPoint = _strategyMap.PlanetPoints[pointIndex];
                HighlightObject(planetPoint.gameObject);

                if (planetPoint.CountSquadOnPoint > 0)
                {
                    var squad = _strategyMap.Squads.FirstOrDefault(s => s.PlanetPoint == planetPoint);
                    if (squad != null)
                        HighlightObject(squad.gameObject);
                }
            }

            if (stage.NeedMoveToPlanet)
            {
                var planetPoint = _strategyMap.PlanetPoints[stage.UnmaskPlanetPointIndexes[0]];
                await UniTask.WaitUntil(() =>
                    planetPoint.HasEnemy ? planetPoint.CountSquadOnPoint > 1 : planetPoint.CountSquadOnPoint > 0);
            }
            else
            {
                foreach (var target in stage.ClickTargets)
                {
                    var clicked = false;
                    var clickHandler = target.AddComponent<ClickHandler>();
                    clickHandler.Clicked += () => clicked = true;
                    await UniTask.WaitUntil(() => clicked);
                }
            }


            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            gameObject.SetActive(false);
            _activeCutoutCounter = 0;

            if (!stage.NeedMoveToPlanet && stage.ClickTargets.Length > 0)
            {
                stage.ClickTargets[0].gameObject.SetActive(stateBefore);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(stage.Delay));
        }

        private void HighlightObject(GameObject obj)
        {
            if (_activeCutoutCounter >= _unmasks.Length)
            {
                Debug.LogError("Not enough cutouts");
                return;
            }

            if (!obj.TryGetComponent(out RectTransform targetRect))
            {
                var go = new GameObject();
                targetRect = go.AddComponent<RectTransform>();
                targetRect.position = Camera.main.WorldToScreenPoint(obj.transform.position);
            }

            if (obj.TryGetComponent(out Image objImage))
                _unmasks[_activeCutoutCounter].GetComponent<Image>().sprite = objImage.sprite;

            _unmasks[_activeCutoutCounter].fitTarget = targetRect;
            _unmasks[_activeCutoutCounter].gameObject.SetActive(true);
            _activeCutoutCounter++;
        }
    }

    [Serializable]
    public struct StrategyTutorialStage
    {
        public float Delay;
        public bool NeedMoveToPlanet;
        [HideIf("NeedMoveToPlanet")]
        public GameObject[] ClickTargets;
        public bool UnmaskEnemy;
        public int[] UnmaskPlanetPointIndexes;
        public RectTransform[] UnmaskObjects;
        public GameObject AppearingObject;
        [TextArea(3, 5)]
        public string Text;
    }
}