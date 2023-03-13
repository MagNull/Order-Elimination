using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    public abstract class FakeAbilityBase : ScriptableObject
    {
        public virtual Sprite Sprite { get; }
        public virtual int Cost { get; }
    }
}
