using System;
using DG.Tweening;
using TMPro;
using UIManagement;
using UnityEngine;
using VContainer;

namespace RoguelikeMap
{
    public class DialogWindow : MonoBehaviour
    {
        private TMP_Text _text;
        private DialogWindowData _data;

        private void Start()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }

        public void SetData(DialogWindowData data)
        {
            _data = data;
            _text.text = _data.Text;
        }

        private void SetWindowByData()
        {
               
        }

        public void PlayAnimation()
        {
            transform.DOMove(_data.TargetOnCanvas, 0.6f).OnComplete(EndAnimation);
        }

        public void EndAnimation()
        {
            transform.DOMove(_data.TargetBehindCanvas, 0.6f).SetDelay(1);
        }
    }
}