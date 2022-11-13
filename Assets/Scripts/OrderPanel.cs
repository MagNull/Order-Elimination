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

        public void SetActive(Squad squad, PlanetPoint planetPoint)
        {
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = true;
            _selectedSquad = squad;
            _selectedPoint = planetPoint;
        }

        public void Disable() 
        {
            GameObject.Find("OrderCanvas").GetComponent<Canvas>().enabled = false;
            _selectedSquad = null;
            _selectedPoint = null;
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
