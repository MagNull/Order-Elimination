using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public interface ISquadMember : ISelectable, IMovable
    {
        public StrategyStats GetStrategyStats();

        public void RaiseExperience(float experience);
    }
}