using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "FakeAbilityInfo", menuName = "TestAbility")]
    public class FakeAbility : FakeAbilityBase
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _cost;
        public override int Cost => _cost;
        public override Sprite Sprite => _sprite;
    }   
}
