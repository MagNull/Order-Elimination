using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoguelikeMap.Points.Models
{
    public class FinalBattlePointModel : BattlePointModel
    {
        public override PointType Type => PointType.Battle;
    }
}