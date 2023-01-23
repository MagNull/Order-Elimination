using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBack : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _activate;
    [SerializeField] private GameObject _disactivate;

    public void OnPointerClick(PointerEventData eventData)
    {
        _activate.SetActive(true);
        _disactivate.SetActive(false);
    }
}
