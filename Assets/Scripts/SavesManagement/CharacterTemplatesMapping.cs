using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    [CreateAssetMenu(fileName = "new Character Template mapping", menuName = "OrderElimination/Project/CharacterTemplatesMapping")]
    public class CharacterTemplatesMapping : SerializedScriptableObject
    {
        private Dictionary<int, CharacterTemplate> _templatesMapping = new();
    }
}
