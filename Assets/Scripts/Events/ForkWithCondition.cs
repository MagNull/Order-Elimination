using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Rendering;
using UnityEngine;

namespace Events
{
    public enum EventCondition
    {
        None,
        Item,
        Code
    }
    
    public class ForkWithCondition : ForkNode
    {
        [SerializeField, TabGroup("Conditions"), HideIf("IsRandom")]
        private List<EventCondition> _conditions;

        [SerializeField, ShowIf("ContainsConditionItem")]
        private List<ItemData> _requirementItems;

        private bool ContainsConditionItem => IsContainsConditionItem();
        private int _count = 0;
        private bool IsContainsConditionItem()
        {
            return _conditions is not null && _conditions.Any(x => x is EventCondition.Item);
        }

        public override void OnEnter(EventPanel panel)
        {
            base.OnEnter(panel);
            var count = 0;
            for (var i = 0; i < _conditions.Count; i++)
            {
                switch (_conditions[i])
                {
                    case EventCondition.Item:
                    {
                        CheckItem(panel, count, i);
                        count++;
                        break;
                    }
                    case EventCondition.Code:
                    {
                        CheckCode(panel, i);
                        break;
                    }
                    case EventCondition.None:
                        continue;
                }
            }
        }

        private void CheckItem(EventPanel panel, int count, int buttonIndex)
        {
            var item = _requirementItems[count];
            if (panel.CheckItem(item))
                return;
            panel.SetInteractableAnswer(buttonIndex, false);
        }

        private void CheckCode(EventPanel panel, int buttonIndex)
        {
            if (!PlayerPrefs.HasKey("Code"))
                panel.SetInteractableAnswer(buttonIndex, false);
        }
    }
}