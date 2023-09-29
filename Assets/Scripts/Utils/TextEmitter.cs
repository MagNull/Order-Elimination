using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace OrderElimination.Utils
{
    //TODO-OPTIMIZE: implement ObjectPool
    public class TextEmitter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textPrefab;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _duration;
        [SerializeField]
        private float _delayBetweenSpawn;
        private float _standardFontSize;
        private float _lastEmitTime = 0;

        public float StandardFontSize => _standardFontSize;

        private void Start()
        {
            _standardFontSize = _textPrefab.fontSize;
        }

        public async UniTask Emit(string text, Color color, Vector3 position, float fontSize = -1)
        {
            await Emit(text, color, position, _speed * _duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, float offsetY = 0, float duration = 1, float fontSize = -1)
        {
            await Emit(text, color, position, new Vector3(0, offsetY, 0), duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, Vector3 offset, float fontSize = -1)
        {
            await Emit(text, color, position, offset, _duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, Vector3 offset, float duration, float fontSize = -1)
        {
            await Emit(text, color, position, offset, 1, duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, Vector3 offset, float scaleOffset, float duration, float fontSize = -1)
        {
            await UniTask.WaitUntil(() => Time.time - _lastEmitTime >= _delayBetweenSpawn);
            _lastEmitTime = Time.time;
            var textInstance = Instantiate(_textPrefab, transform);
            textInstance.transform.position = position;
            textInstance.gameObject.SetActive(true);
            textInstance.fontSize = fontSize > 0 ? fontSize : _standardFontSize;
            textInstance.text = text;
            textInstance.color = color;
            textInstance.DOComplete();
            textInstance.transform.DOComplete();
            textInstance.transform.DOMove(position + offset, duration);
            var endScale = textInstance.transform.localScale * scaleOffset;
            textInstance.transform.DOScale(endScale, duration);
            textInstance.DOFade(0, duration).SetEase(Ease.InFlash).OnComplete(() => Destroy(textInstance.gameObject));
        }

        public async UniTask Emit(TextEmitterContext context)
        {
            await UniTask.WaitUntil(() => Time.time - _lastEmitTime >= _delayBetweenSpawn);
            _lastEmitTime = Time.time;
            var textInstance = Instantiate(_textPrefab, transform);
            textInstance.gameObject.SetActive(true);
            textInstance.transform.position = context.Origin;
            //textInstance.fontSize = fontSize > 0 ? fontSize : _standardFontSize;
            textInstance.text = context.Text;
            textInstance.color = context.TextColor;
            textInstance.outlineColor = context.OutlineColor;
            textInstance.outlineWidth = context.OutlineWidth;
            textInstance.DOComplete();
            textInstance.transform.DOComplete();
            textInstance.transform
                .DOMove(context.Origin + context.Offset, context.TotalTime)
                .SetEase(context.OffsetMoveEase);
            //var endScale = textInstance.transform.localScale * scaleOffset;
            //textInstance.transform.DOScale(endScale, duration);
            var fadeSequence = DOTween.Sequence(textInstance)
                .Append(textInstance.DOFade(0, 0))
                .Append(textInstance.DOFade(1, context.AppearTime).SetEase(context.AppearEase))
                .AppendInterval(context.HoldTime)
                .Append(textInstance.DOFade(0, context.DisappearTime).SetEase(context.DisappearEase))
                .OnComplete(() => Destroy(textInstance.gameObject));
            await fadeSequence.AsyncWaitForCompletion();
        }
    }

    public struct TextEmitterContext
    {
        public static TextEmitterContext Default => new()
        {
            Text = "New text",
            TextColor = Color.white,
            OutlineColor = Color.black,
            OutlineWidth = 0,
            Origin = Vector3.zero,
            Offset = Vector3.zero,
            OffsetMoveEase = Ease.Flash,
            AppearTime = 0.25f,
            HoldTime = 1,
            DisappearTime = 0.25f,
            AppearEase = Ease.InFlash,
            DisappearEase = Ease.InFlash,
        };

        [BoxGroup("Text Properties", CenterLabel = true)]
        [MultiLineProperty]
        [ShowInInspector, SerializeField]
        public string Text { get; set; }

        [BoxGroup("Text Properties")]
        [ShowInInspector, SerializeField]
        public Color TextColor { get; set; }

        [BoxGroup("Text Properties")]
        [ShowInInspector, SerializeField]
        public float OutlineWidth { get; set; }

        [BoxGroup("Text Properties")]
        [ShowInInspector, SerializeField]
        public Color OutlineColor { get; set; }

        [BoxGroup("Movement", CenterLabel = true)]
        [ShowInInspector, SerializeField]
        public Vector3 Origin { get; set; }

        [BoxGroup("Movement")]
        [ShowInInspector, SerializeField]
        public Vector3 Offset { get; set; }

        [BoxGroup("Movement")]
        [ShowInInspector, SerializeField]
        public Ease OffsetMoveEase { get; set; }

        [BoxGroup("Timing", CenterLabel = true)]
        [ShowInInspector]
        public float TotalTime => AppearTime + HoldTime + DisappearTime;

        [BoxGroup("Timing")]
        [MinValue(0)]
        [ShowInInspector, SerializeField]
        public float AppearTime { get; set; }

        [BoxGroup("Timing")]
        [ShowInInspector, SerializeField]
        public float HoldTime { get; set; }

        [BoxGroup("Timing")]
        [MinValue(0)]
        [ShowInInspector, SerializeField]
        public float DisappearTime { get; set; }

        [BoxGroup("Timing")]
        //[EnableIf("@" + nameof(AppearTime) + ">0")]
        [ShowInInspector, SerializeField]
        public Ease AppearEase { get; set; }

        [BoxGroup("Timing")]
        //[EnableIf("@" + nameof(DisappearTime) + ">0")]
        [ShowInInspector, SerializeField]
        public Ease DisappearEase { get; set; }

    }
}