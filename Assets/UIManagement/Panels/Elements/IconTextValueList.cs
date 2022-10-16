using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class IconTextValueList: MonoBehaviour
    {
        private List<IconTextValueElement> _elements = new List<IconTextValueElement>();
        [SerializeField] private RectTransform _transform;

        //private float _iconSize = 64;
        //public float IconSize 

        public void AddElement(Sprite icon, string text, string value)
        {
            _elements.Add(UICommonElementsBuilder.CreateIconTextValueElement(icon, text, value, _transform));
        }

        private void ChangeIconSize(float size)
        {

        }
    }
}
