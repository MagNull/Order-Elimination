using UnityEngine;
using UnityEngine.UI;
using System;
using VContainer;

namespace OrderElimination
{
    public class OrderPanel : MonoBehaviour
    {
        private SquadCommander _squadCommander;

        [Inject]
        private void Construct(SquadCommander squadCommander)
        {
            _squadCommander = squadCommander;
        }
        
        private void Start() 
        {
            InputClass.TargetSelected += SetOrder;
            Disable();
        }

        public void SetActive()
        {
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = true;
        }

        public void Disable() 
        {
            if(_squadCommander.Squad != null)
                _squadCommander.Squad.SetOrderButtonCharacteristics(false);
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = false;
        }

        public void SetOrder(Squad squad, PlanetPoint planetPoint)
        {
            _squadCommander.Set(squad, planetPoint);
            squad.SetOrderButtonCharacteristics(true);
        }

        public void ResearchButtonIsClicked()
        {
            _squadCommander.CreateResearchOrder();
            Disable();
        }

        public void AttackButtonIsClicked()
        {
            _squadCommander.CreateAttackOrder();
            Disable();
        }
    }
}
