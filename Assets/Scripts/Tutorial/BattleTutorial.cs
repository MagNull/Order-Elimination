using System;
using System.Threading.Tasks;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class BattleTutorial : SerializedMonoBehaviour
    {
        [SerializeField]
        private BattleTutorialStage[] _tutorialStages;
        [SerializeField]
        private Unmask[] _unmasks;

        [SerializeField]
        private BattleMapView _battleMapView;
        [SerializeField]
        private TextMeshProUGUI _tutorialText;

        private int _activeCutoutCounter;

        [Button]
        private void ResetTutorial()
        {
            PlayerPrefs.SetInt("Battle Tutorial", 1);
        }
        
        
        private async void Start()
        {
            if (PlayerPrefs.GetInt("Battle Tutorial") < 0)
            {
                gameObject.SetActive(false);
                return;
            }
            PlayerPrefs.SetInt("Battle Tutorial", -1);
            await StartTutorial();
        }

        private async UniTask StartTutorial()
        {
            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            gameObject.SetActive(false);
            await UniTask.Delay(400);
            foreach (var tutorialStage in _tutorialStages)
            {
                await ProceedStage(tutorialStage);
            } }

        private async Task ProceedStage(BattleTutorialStage stage)
        {
            gameObject.SetActive(true);
            _tutorialText.text = stage.Text;
            stage.AppearingObject.SetActive(true);

            var cells = stage.UnmaskCells;
            for (var i = 0; i < cells.Length; i++)
            {
                CellView cell = _battleMapView.GetCell(cells[i].x, cells[i].y);
                HighlightPosition(cell.gameObject);
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
            _activeCutoutCounter = 0;
            await UniTask.Delay(TimeSpan.FromSeconds(stage.Delay));
        }
        
        private void HighlightPosition(GameObject obj)
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
    public struct BattleTutorialStage
    {
        public Vector2Int ClickTarget;
        public int ClickCount;
        public float Delay;
        public Vector2Int[] UnmaskCells;
        public RectTransform[] UnmaskUIs;
        public GameObject AppearingObject;
        [TextArea(3, 5)]
        public string Text;
    }
}