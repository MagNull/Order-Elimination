using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    public class SquadView
    {
        private Transform _transform;
        private Button _buttonOnOrderPanelTransform;

        public SquadView(Transform transform)
        {
            _transform = transform;
        }

        public void SetButtonOnOrder(Button button)
        {
            _buttonOnOrderPanelTransform = button;
            SetButtonCharacteristics(false);
        }

        public void SetButtonCharacteristics(bool isActive)
        {
            _buttonOnOrderPanelTransform.interactable = isActive;
            _buttonOnOrderPanelTransform.GetComponentInChildren<SpriteRenderer>().enabled = isActive;
            _buttonOnOrderPanelTransform.GetComponentInChildren<Text>().enabled = isActive;
        }

        public void OnMove(PlanetPoint planetPoint)
        {
            _transform.position = planetPoint.transform.position + new Vector3(-20f, 50f);
        }

        public void OnSelect()
        {
            _transform.localScale += new Vector3(0.1f, 0.1f, 0);
            _buttonOnOrderPanelTransform.transform.localScale += new Vector3(0.1f, 0.1f, 0);
        }

        public void OnUnselect()
        {
            _transform.localScale -= new Vector3(0.1f, 0.1f, 0);
            _buttonOnOrderPanelTransform.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
        }
    }
}
