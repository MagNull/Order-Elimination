using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class EventPoint : OrderElimination.Point
    {
        public string Text { get; set; }
        public override void Visit(Squad squad)
        {
            squad.VisitEventPoint(this);
        }
    }
}