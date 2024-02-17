using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace Events
{
    enum HPRegulationType
    {
        Heal,
        Damage
    }

    [NodeWidth(300)]
    public class HPRegulationNode : EventNode
    {
        [SerializeField]
        private HPRegulationType _regulationType;

        [SerializeField]
        private int _hpPercentage;

        public override void OnEnter(EventPanel panel)
        {
            base.OnEnter(panel);
            switch (_regulationType)
            {
                case HPRegulationType.Heal:
                    panel.HealCharacters(_hpPercentage);
                    break;
                case HPRegulationType.Damage:
                    panel.DamageCharacters(_hpPercentage);
                    break;
            }
        }
    }
}