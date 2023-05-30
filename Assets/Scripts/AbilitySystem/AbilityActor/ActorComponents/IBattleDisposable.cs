﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleDisposable
    {
        public event Action<IBattleDisposable> DisposedFromBattle;

        public bool DisposeFromBattle();
    }
}
