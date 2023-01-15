using System;
using System.Threading.Tasks;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using OrderElimination.BattleMap;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tutorial
{
    public class BattleTutorial : SerializedMonoBehaviour
    {
        [SerializeField]
        private BattleTutorialStage[] _tutorialStages;
        [SerializeField]
        private RectTransform[] _cutouts;
        [SerializeField]
        private Unmask[] _unmasks;

        [SerializeField]
        private BattleMapView _battleMapView;

        private int _activeCutoutCounter;

        private async void Start()
        {
            if (PlayerPrefs.GetInt("Tutorial") == -1)
            {
                gameObject.SetActive(false);
                return;
            }
            await StartTutorial();
        }

        private async UniTask StartTutorial()
        {
            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            gameObject.SetActive(false);
            await UniTask.Delay(300);
            foreach (var tutorialStage in _tutorialStages)
            {
                await ProceedStage(tutorialStage);
            }
        }

        private async Task ProceedStage(BattleTutorialStage stage)
        {
            gameObject.SetActive(true);
            var cells = stage.UnmaskCells;
            for (var i = 0; i < cells.Length; i++)
            {
                CellView cell = _battleMapView.GetCell(cells[i].x, cells[i].y);
                var screenPos = Camera.main.WorldToScreenPoint(cell.transform.position);
                HighlightPosition(screenPos);
            }

            var uis = stage.UnmaskUIs;
            for (var i = 0; i < uis.Length; i++)
            {
                var unmask = _unmasks[i + stage.UnmaskCells.Length];
                unmask.fitTarget = uis[i];
                unmask.gameObject.SetActive(true);
            }

            CellView finishTargetCell = _battleMapView.GetCell(stage.ClickTarget.x, stage.ClickTarget.y);
            var clickCounter = 0;
            finishTargetCell.CellClicked += _ => clickCounter++;
            await UniTask.WaitUntil(() => stage.ClickCount <= clickCounter);
            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(stage.Delay));
        }
        
        private void HighlightPosition(Vector3 pos)
        {
            if (_activeCutoutCounter >= _cutouts.Length)
            {
                Debug.LogError("Not enough cutouts");
                return;
            }
            _cutouts[_activeCutoutCounter].transform.position = pos;
            _unmasks[_activeCutoutCounter].fitTarget = _cutouts[_activeCutoutCounter];
            _unmasks[_activeCutoutCounter].gameObject.SetActive(true);
            _activeCutoutCounter++;
        }
    }

    [Serializable]
    public struct BattleTutorialStage
    {
        public Vector2Int ClickTarget;
        public int ClickCount;
        public float Delay;
        public Vector2Int[] UnmaskCells;
        public RectTransform[] UnmaskUIs;
        public GameObject AppearingObjects;
    }
}