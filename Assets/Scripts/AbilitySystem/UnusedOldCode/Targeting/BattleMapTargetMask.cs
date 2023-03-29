using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public struct BattleMapTargetMask
    {
        public bool EmptyCells;
        public bool EnemyCharacters;
        public bool EnemyStructures;
        public bool FriendlyCharacters;
        public bool FriendlyStructures;
        public bool NeutralStructures;
    }
}
