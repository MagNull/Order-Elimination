using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using OrderElimination;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Events
{
    public enum EventCondition
    {
        None,
        Item,
        Character
    }
    
    public class ForkWithCondition : ForkNode
    {
        [SerializeField, TabGroup("Conditions"), HideIf("IsRandom")]
        private List<EventCondition> _conditions;

        [SerializeField, ShowIf("ContainsConditionItem")]
        private List<ItemData> _requirementItems;
        
        [SerializeField, ShowIf("ContainsConditionCharacter")]
        private List<CharacterTemplate> _requirementCharacter;

        private int _itemCounter = 0;
        private int _characterCounter = 0;

        private bool ContainsConditionCharacter => IsContainsConditionCharacter();
        private bool ContainsConditionItem => IsContainsConditionItem();
        private int _count = 0;
        private bool IsContainsConditionItem()
        {
            return _conditions is not null && _conditions.Any(x => x is EventCondition.Item);
        }
        
        private bool IsContainsConditionCharacter()
        {
            return _conditions is not null && _conditions.Any(x => x is EventCondition.Character);
        }

        public override void OnEnter(EventPanel panel)
        {
            base.OnEnter(panel);
            _itemCounter = 0;
            _characterCounter = 0;
            for (var i = 0; i < _conditions.Count; i++)
            {
                switch (_conditions[i])
                {
                    case EventCondition.Item:
                    {
                        CheckItem(panel, i);
                        continue;
                    }
                    case EventCondition.Character:
                    {
                        CheckCharacter(panel, i);
                        continue;
                    }
                    case EventCondition.None:
                        continue;
                }
            }
        }

        private void CheckItem(EventPanel panel, int buttonIndex)
        {
            var item = _requirementItems[_itemCounter];
            _itemCounter++;
            if (panel.CheckItem(item))
                return;
            panel.SetInteractableAnswer(buttonIndex, false);
        }

        private void CheckCharacter(EventPanel panel, int buttonIndex)
        {
            var character = _requirementCharacter[_characterCounter];
            _characterCounter++;
            if (panel.CheckCharacter(character))
                return;
            panel.SetInteractableAnswer(buttonIndex, false);
        }
    }
}