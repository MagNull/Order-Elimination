using System;
using System.Collections.Generic;
using Events;
using RoguelikeMap.Panels;
using RoguelikeMap.Points.Models;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using UnityEngine;
using XNode;

namespace RoguelikeMap.Points
{
    [Serializable]
    public class StartPointModel : PointModel
    {
        [Output] public PointModel exits;
    }
}