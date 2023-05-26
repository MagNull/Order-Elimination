using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Rework
{
    public class NoTargetAbility
    {
        public bool Execute(AbilitySystemActor caster)
        {
            throw new NotImplementedException();
        }
    }

    public class PointTargetAbility
    {
        public bool Execute(AbilitySystemActor caster, Vector2Int target)
        {
            throw new NotImplementedException();
        }
    }

    public class UnitTargetAbility
    {
        public bool Execute(AbilitySystemActor caster, AbilitySystemActor target)
        {
            throw new NotImplementedException();
        }
    }
}
