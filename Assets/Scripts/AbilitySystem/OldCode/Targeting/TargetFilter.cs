﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TargetFilter
    {
        public BattleMapTargetMask TargetMask;
        public bool IsAccepted(Cell targetingCell)
        {
            throw new NotImplementedException();
        }

        public bool IsAccepted(IBattleEntity targetingEntity)
        {
            throw new NotImplementedException();
        }
    }
}
