using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    public class SquadView
    {
        public static int count = 0;
        private Transform _transform;
        private Image __rectanglebOnPanelButton;

        public SquadView(Transform transform)
        {
            _transform = transform;
        }

        public void SetButtonOnOrder(Image image)
        {
            __rectanglebOnPanelButton = image;
            SetButtonCharacteristics(false);
        }

        public void SetButtonCharacteristics(bool isActive)
        {
            __rectanglebOnPanelButton.enabled = isActive;
            __rectanglebOnPanelButton.GetComponentInChildren<Button>().interactable = isActive;
            __rectanglebOnPanelButton.GetComponentInChildren<Text>().enabled = isActive;
        }

        public void OnMove(PlanetPoint planetPoint)
        {
            _transform.position = planetPoint.transform.position + new Vector3(-50 + (planetPoint.CountSquadOnPoint - 1) * 100f, 60f);
        }

        public void OnSelect()
        {
            //_transform.localScale += new Vector3(0.1f, 0.1f, 0);
            //_buttonOnOrderPanelTransform.transform.localScale += new Vector3(0.1f, 0.1f, 0);
        }

        public void OnUnselect()
        {
            //_transform.localScale -= new Vector3(0.1f, 0.1f, 0);
            //_buttonOnOrderPanelTransform.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
        }
    }
}
