using DG.Tweening;
using UnityEngine;
using VContainer;

namespace RoguelikeMap
{
    public class DialogWindow : MonoBehaviour
    {
        private DialogWindowData _data;

        public void SetData(DialogWindowData data)
        {
            _data = data;
        }

        private void SetWindowByData()
        {
               
        }

        public void PlayAnimation()
        {
            transform.DOMove(_data.TargetOnCanvas, 0.6f);
        }

        public void EndAnimation()
        {
            transform.DOMove(_data.TargetBehindCanvas, 0.6f);
        }
    }
}