using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public interface ISquadMember : ISelectable, IMovable
    {
        public int GetStats();

        public void RaiseExpirience(float expirience);
    }
}