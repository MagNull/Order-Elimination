using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrderElimination.Utils
{
    //TODO-OPTIMIZE: implement ObjectPool
    public class TextEmitter : SerializedMonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textPrefab;

        [SerializeField]
        private float _mininalTimeInterval;

        [SerializeField]
        private int _sortingOrder;

        [ShowInInspector, OdinSerialize]
        private TextEmitterContext _defaultTextStyle = TextEmitterContext.Default;

        private float _lastEmitTime = 0;

        public async UniTask Emit(string text)
        {
            Debug.Log("Emit Simple");
            var context = _defaultTextStyle;
            context.Text = text;
            await Emit(context);
        }

        public async UniTask Emit(string text, Vector3 position)
        {
            Debug.Log("Emit Button");
            var context = _defaultTextStyle;
            context.Text = text;
            context.Origin = position;
            await Emit(context);
        }

        public async UniTask Emit(TextEmitterContext context)
        {
            await UniTask.WaitUntil(() => Time.time - _lastEmitTime >= _mininalTimeInterval);
            _lastEmitTime = Time.time;
            var textInstance = Instantiate(_textPrefab, transform);
            textInstance.gameObject.SetActive(true);
            textInstance.canvas.sortingOrder = _sortingOrder;
            textInstance.transform.position = context.Origin;
            textInstance.fontSize = context.FontSize;
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

        [Button]
        private void TestEmit() => Emit(_defaultTextStyle);
    }

    public struct TextEmitterContext
    {
        public static TextEmitterContext Default => new()
        {
            Text = "New text",
            FontSize = 1,
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
        [MinValue(0)]
        [ShowInInspector, SerializeField]
        public float FontSize { get; set; }

        [BoxGroup("Text Properties")]
        [ShowInInspector, SerializeField]
        public Color TextColor { get; set; }

        [BoxGroup("Text Properties")]
        [ShowInInspector, SerializeField]
        public float OutlineWidth { get; set; }

        [BoxGroup("Text Properties")]
        [ShowInInspector, SerializeField]
        public Color OutlineColor { get; set; }

        [BoxGroup("Text Movement", CenterLabel = true)]
        [ShowInInspector, SerializeField]
        public Vector3 Origin { get; set; }

        [BoxGroup("Text Movement")]
        [ShowInInspector, SerializeField]
        public Vector3 Offset { get; set; }

        [BoxGroup("Text Movement")]
        [ShowInInspector, SerializeField]
        public Ease OffsetMoveEase { get; set; }

        [BoxGroup("Timings", CenterLabel = true)]
        [ShowInInspector]
        public float TotalTime => AppearTime + HoldTime + DisappearTime;

        [BoxGroup("Timings")]
        [MinValue(0)]
        [ShowInInspector, SerializeField]
        public float AppearTime { get; set; }

        [BoxGroup("Timings")]
        [ShowInInspector, SerializeField]
        public float HoldTime { get; set; }

        [BoxGroup("Timings")]
        [MinValue(0)]
        [ShowInInspector, SerializeField]
        public float DisappearTime { get; set; }

        [BoxGroup("Timings")]
        //[EnableIf("@" + nameof(AppearTime) + ">0")]
        [ShowInInspector, SerializeField]
        public Ease AppearEase { get; set; }

        [BoxGroup("Timings")]
        //[EnableIf("@" + nameof(DisappearTime) + ">0")]
        [ShowInInspector, SerializeField]
        public Ease DisappearEase { get; set; }

    }
}