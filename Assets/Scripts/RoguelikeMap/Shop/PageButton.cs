using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PageButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    private ShopPage _page;
    public bool isActivePage;
    private static List<PageButton> _buttons = new List<PageButton>();
    
    public void Initialize(ShopPage page, string name)
    {
        _page = page;
        _buttons.Add(this);
        _label.text = name;
        GetComponent<Button>().onClick.AddListener(Click);
        if (_buttons.All(x => !x.isActivePage))
            Click();   
    }

    public void Click()
    {
        foreach (var x in _buttons.Where(x => x.isActivePage))
        {
            x._page.ChangeVisibility();
            x.isActivePage = false;
        }
        _page.ChangeVisibility();
        isActivePage = true;
    }
}
