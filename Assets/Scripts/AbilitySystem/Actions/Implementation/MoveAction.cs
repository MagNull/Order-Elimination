using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class MoveAction : BattleAction<MoveAction>
    {
        public override ActionTargets ActionTargets => ActionTargets.CellsOnly;

        protected override bool Perform(ActionExecutionContext useContext)
        {
            useContext.ActionMaker.Move(useContext.ActionTargetPosition);
            return true;
        }
    }
}
