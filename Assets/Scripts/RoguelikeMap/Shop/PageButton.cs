using System;
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
    public bool isActivePage => _page.isActiveAndEnabled;
    private static List<PageButton> _buttons = new ();
    
    public void Initialize(ShopPage page, string name)
    {
        _page = page;
        _label.text = name;
        GetComponent<Button>().onClick.AddListener(() => Click(_page) );
        _buttons.Add(this);
        if (_buttons?.Count(x => x.isActivePage)>1)
            Click(page);

    }

    public void Click(ShopPage page)
    {
        foreach (var x in _buttons.Where(x => x.isActivePage))
            x._page.ChangeVisibility(false);
        page.ChangeVisibility(true);
    }

    private void OnDestroy()
    {
        _buttons.Remove(this);
    }
}
