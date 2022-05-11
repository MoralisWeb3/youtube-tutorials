using System;
using UnityEngine;

public class ShopItem : InventoryItem
{
    public static event Action<InventoryItem> Selected;

    public void OnClickHandler()
    {
        Selected?.Invoke(this);
    }
}
