using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIManagement.Elements
{
    public static class UICommonElementsBuilder
    {
        private static PrefabHolder _prefabHolder;

        public static void Initialize()
        {
            if (_prefabHolder == null)
            {
                var holder = GameObject.FindObjectOfType<PrefabHolder>();
                if (holder == null)
                    throw new Exception($"No {nameof(PrefabHolder)} in scene");
                _prefabHolder = holder;
            }
        }

        public static IconTextValueElement CreateIconTextValueElement(Sprite icon, string text, string value, Transform parent)
        {
            var element = GameObject.Instantiate(_prefabHolder._iconTextValueElementPrefab, parent);
            element.Icon = icon;
            element.Text = text;
            element.Value = value;
            return element;
        }
    }
}
