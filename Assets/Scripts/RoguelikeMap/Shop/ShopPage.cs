using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;
using UnityEngine.Serialization;

public class ShopPage : MonoBehaviour
{
    [SerializeField] private ShopItem _itemPrefab;

    public void CreateItems(List<FakeAbilityBase> abilities)
    {
        foreach (var ability in abilities)
            AddItem(ability);
    }

    public void AddItem(FakeAbilityBase ability)
    {
        var item = Instantiate(_itemPrefab, transform);
        item.Initialize(ability);
    }

    public void ChangeVisibility()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
