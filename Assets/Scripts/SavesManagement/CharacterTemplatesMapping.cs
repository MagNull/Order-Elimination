using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    [CreateAssetMenu(fileName = "new Character Template mapping", menuName = "OrderElimination/Project/CharacterTemplatesMapping")]
    public class CharacterTemplatesMapping : 
        SerializedScriptableObject, 
        IDataMapping<int, IGameCharacterTemplate>
    {
        [OdinSerialize]
        private Dictionary<int, IGameCharacterTemplate> _templatesMapping = new();

        [OdinSerialize]
        private Dictionary<IGameCharacterTemplate, int> _keysMapping = new();

        [Button]
        public void AddTemplatesToMapping(IEnumerable<IGameCharacterTemplate> templates)
        {
            foreach (var template in templates)
            {
                AddTemplateToMapping(template);
            }
        }

        public bool AddTemplateToMapping(IGameCharacterTemplate template)
        {
            if (_keysMapping.ContainsKey(template))
                return false;
            var insertKey = GetNextAvailableKey();
            if (_templatesMapping.ContainsKey(insertKey))
                throw new NotSupportedException("Key already exists.");
            _templatesMapping.Add(insertKey, template);
            _keysMapping.Add(template, insertKey);
            return true;
        }

        public bool RemoveTemplateFromMapping(IGameCharacterTemplate template)
        {
            if (!_keysMapping.ContainsKey(template))
                return false;
            var key = _keysMapping[template];
            _keysMapping.Remove(template);
            _templatesMapping.Remove(key);
            return true;
        }

        public void Clear()
        {
            _keysMapping.Clear();
            _templatesMapping.Clear();
        }

        private int GetNextAvailableKey()
        {
            var occupiedKeys = _templatesMapping.Keys.OrderBy(k => k).ToArray();
            for (var i = 0; i < occupiedKeys.Length - 1; i++)
            {
                if (occupiedKeys[i + 1] > occupiedKeys[i] + 1)
                    return occupiedKeys[i] + 1;
            }
            return occupiedKeys[occupiedKeys.Length - 1] + 1;
        }

        public IGameCharacterTemplate GetData(int key) => _templatesMapping[key];

        public int GetKey(IGameCharacterTemplate data) => _keysMapping[data];
    }
}
