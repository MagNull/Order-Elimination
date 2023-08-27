using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    //[CreateAssetMenu(fileName = "new Character Templates mapping", menuName = "OrderElimination/Project/CharacterTemplatesMapping")]
    public class CharacterTemplatesMapping : 
        SerializedScriptableObject, 
        IDataMapping<int, IGameCharacterTemplate>
    {
        [HideInInspector, OdinSerialize]
        private Dictionary<int, IGameCharacterTemplate> _templatesMapping = new();

        [HideInInspector, OdinSerialize]
        private Dictionary<IGameCharacterTemplate, int> _keysMapping = new();

        [ShowInInspector]
        public IReadOnlyDictionary<int, IGameCharacterTemplate> MappedTemplates => _templatesMapping;

        [Button]
        public void AddTemplatesToMapping(IGameCharacterTemplate[] templates)
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

        public bool RemoveFromMapping(IGameCharacterTemplate template)
        {
            if (!_keysMapping.ContainsKey(template))
                return false;
            var key = _keysMapping[template];
            _keysMapping.Remove(template);
            _templatesMapping.Remove(key);
            return true;
        }

        [Button]
        public bool RemoveFromMapping(int key)
        {
            if (!_templatesMapping.ContainsKey(key))
                return false;
            return RemoveFromMapping(_templatesMapping[key]);
        }

        public void Clear()
        {
            _keysMapping.Clear();
            _templatesMapping.Clear();
        }

        private int GetNextAvailableKey()
        {
            var startingKey = 0;
            if (_keysMapping.Count == startingKey)
                return 0;
            if (_keysMapping.Count == 1) 
                return 1;
            var occupiedKeys = _templatesMapping.Keys.OrderBy(k => k).ToArray();
            if (occupiedKeys.Length > 0 && occupiedKeys[0] > startingKey)
                return startingKey;
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
