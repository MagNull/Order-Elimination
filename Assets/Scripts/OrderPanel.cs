using UnityEngine;
using UnityEngine.UI;
using System;
using VContainer;

namespace OrderElimination
{
    public class OrderPanel : MonoBehaviour
    {
        private Squad _selectedSquad;
        private PlanetPoint _selectedPoint;
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
            if(_selectedSquad != null)
                _selectedSquad.SetOrderButtonCharacteristics(false);
            _selectedSquad = null;
            _selectedPoint = null;
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = false;
        }

        public void SetOrder(Squad squad, PlanetPoint planetPoint)
        {
            _selectedSquad = squad;
            _selectedPoint = planetPoint;
            _selectedSquad.SetOrderButtonCharacteristics(true);
        }

        public void ResearchButtonIsClicked()
        {
            _selectedSquad.SetOrder(_squadCommander.CreateResearchOrder(_selectedPoint, _selectedSquad));
            Disable();
        }

        public void AttackButtonIsClicked()
        {
            _selectedSquad.SetOrder(_squadCommander.CreateAttackOrder(_selectedPoint, _selectedSquad));
            Disable();
        }
    }
}
