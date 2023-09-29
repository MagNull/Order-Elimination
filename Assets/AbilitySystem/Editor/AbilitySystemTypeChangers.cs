using OrderElimination.AbilitySystem.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.Editor
{
    public class AbilitySystemTypeChangers
    {
        public void ChangeWalkAnimationInstance(WalkAnimation animation)
        {
            Debug.Log("Animation changed");
        }

        public void ChangeWalkAnimationInstance(object animation)
        {
            Debug.Log("Animation changed");
        }
    }
}
