using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIManagement.Elements
{
    public class PrefabHolder : MonoBehaviour
    {
        [SerializeField] public IconTextValueElement _iconTextValueElementPrefab;
        [SerializeField] public CharacterListElement _characterListElementElementPrefab;
    }
}
