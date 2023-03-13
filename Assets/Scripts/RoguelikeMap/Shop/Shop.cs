using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] 
    private ShopPage _pagePrefab;
    [SerializeField] 
    private PageButton _buttonPrefab;
    [SerializeField] 
    private Transform _buttonsTransform;

    private List<ShopPage> _pages = new List<ShopPage>();
    
    public void CreateShopPage(string pageName, List<FakeAbilityBase> abilityBases)
    {
        var page = Instantiate(_pagePrefab, transform);
        page.CreateItems(abilityBases);
        page.ChangeVisibility();
        _pages.Add(page);
        var button = Instantiate(_buttonPrefab, _buttonsTransform);
        button.Initialize(page, pageName);
    }

    public static void Buy(ShopItem item)
    {
        var abil = item.Ability;
        //место для добавленя в инвентарь и покупки
        Debug.Log("Cost: " + item.Cost);
        Destroy(item.gameObject);
    }
    
    // заглушка
    private void Start()
    {
        var abilities = Resources.LoadAll<FakeAbility>("TestAbility");
        CreateShopPage("Предметы", abilities.Select(x=>(FakeAbilityBase)x).ToList());
        CreateShopPage("Абилки", abilities.Select(x=>(FakeAbilityBase)x).Reverse().ToList());
    }
    // CreateShop() ?
    // ClearShop() ?
}
