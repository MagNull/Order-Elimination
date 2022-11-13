using UnityEngine;
using UnityEngine.UI;
using System;

namespace OrderElimination
{
    public class OrderPanel : MonoBehaviour
    {
        private Squad _selectedSquad;
        private PlanetPoint _selectedPoint;
        
        private void Start() 
        {
            Disable();
        }

        public void SetActive()
        {
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = true;
        }

        public void Disable() 
        {
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = false;
            if(_selectedSquad != null)
                _selectedSquad.SetOrderButtonCharacteristics(false);
            _selectedSquad = null;
            _selectedPoint = null;
        }

        public void SetOrder(Squad squad, PlanetPoint planetPoint)
        {
            _selectedSquad = squad;
            _selectedPoint = planetPoint;
            _selectedSquad.SetOrderButtonCharacteristics(true);
        }

        public void ResearchButtonIsClicked()
        {
            _selectedSquad.SetOrder(SquadCommander.CreateResearchOrder(_selectedPoint, _selectedSquad));
            _selectedSquad.Move(_selectedPoint);
            Disable();
        }

        public void AttackButtonIsClicked()
        {
            _selectedSquad.SetOrder(SquadCommander.CreateAttackOrder(_selectedPoint, _selectedSquad));
            _selectedSquad.Move(_selectedPoint);
            Disable();
        }
    }
}
