using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using OrderElimination;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

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
        private int _activeCutoutCounter = 0;

        private async void Start()
        {
            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            _strategyMap.SpawnEnemy(2);
            gameObject.SetActive(false);
            await UniTask.Delay(300);
            gameObject.SetActive(true);
            foreach (var stage in _tutorialStages)
            {
                if (stage.UnmaskEnemy)
                {
                    HighlightPosition(_strategyMap.EnemySquad.GetComponent<RectTransform>());
                }

                foreach (var obj in stage.UnmaskObjects)
                {
                    HighlightPosition(obj);
                }

                foreach (var pointIndex in stage.UnmaskPlanetPointIndexes)
                {
                    HighlightPosition(_strategyMap.PlanetPoints[pointIndex].GetComponent<RectTransform>());
                }

                var clickCounter = new Dictionary<GameObject, int>();
                foreach (var target in stage.ClickTargets)
                {
                    var obj = target.Key;
                    var count = target.Value;
                    var clickHandler = obj.AddComponent<ClickHandler>();
                    clickHandler.Clicked += () =>
                    {
                        if (clickCounter.ContainsKey(obj))
                        {
                            clickCounter[obj]++;
                        }
                        else
                        {
                            clickCounter.Add(obj, 1);
                        }
                    };
                    await UniTask.WaitUntil(() =>
                    {
                        if (clickCounter.ContainsKey(obj))
                        {
                            return clickCounter[obj] >= count;
                        }

                        return false;
                    });
                }

                await UniTask.Delay(TimeSpan.FromSeconds(stage.Delay));
            }
        }

        private void HighlightPosition(RectTransform obj)
        {
            if (_activeCutoutCounter >= _unmasks.Length)
            {
                Debug.LogError("Not enough cutouts");
                return;
            }

            _unmasks[_activeCutoutCounter].fitTarget = obj;
            _unmasks[_activeCutoutCounter].gameObject.SetActive(true);
            _activeCutoutCounter++;
        }
    }

    [Serializable]
    public struct StrategyTutorialStage
    {
        public float Delay;
        public Dictionary<GameObject, int> ClickTargets;
        public bool UnmaskEnemy;
        public int[] UnmaskPlanetPointIndexes;
        public RectTransform[] UnmaskObjects;
        public GameObject[] AppearingObjects;
    }
}