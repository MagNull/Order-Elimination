using System;
using System.Threading.Tasks;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using OrderElimination;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class StandardTutorial : SerializedMonoBehaviour
    {
        [SerializeField]
        private GameObject _triggerObject;
        [SerializeField]
        private StandardTutorialStage[] _tutorialStages;
        [SerializeField]
        private Unmask[] _unmasks;
        [SerializeField]
        private TextMeshProUGUI _tutorialText;
        private int _activeCutoutCounter = 0;

        
        [Button]
        private void ResetTutorial()
        {
            PlayerPrefs.SetInt("Standard Tutorial", 1);
        }
        
        private async void Start()
        {
            if (PlayerPrefs.GetInt("Standard Tutorial") < 0)
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
            await UniTask.WaitUntil(() => _triggerObject.activeSelf);
            foreach (var stage in _tutorialStages)
            {
                await ProceedStages(stage);
            }
            PlayerPrefs.SetInt("Standard Tutorial", -1);
        }

        private async Task ProceedStages(StandardTutorialStage stage)
        {
            gameObject.SetActive(true);
            _tutorialText.text = stage.Text;
            stage.AppearingObject.SetActive(true);
            foreach (var obj in stage.UnmaskObjects)
            {
                HighlightObject(obj.gameObject);
            }

            foreach (var target in stage.ClickTargets)
            {
                var clicked = false;
                var clickHandler = target.AddComponent<ClickHandler>();
                clickHandler.Clicked += () => clicked = true;
                await UniTask.WaitUntil(() => clicked);
            }

            _unmasks.ForEach(u => u.gameObject.SetActive(false));
            gameObject.SetActive(false);
            _activeCutoutCounter = 0;

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
    public struct StandardTutorialStage
    {
        public float Delay;
        public GameObject[] ClickTargets;
        public RectTransform[] UnmaskObjects;
        public GameObject AppearingObject;
        [TextArea]
        public string Text;
    }
}