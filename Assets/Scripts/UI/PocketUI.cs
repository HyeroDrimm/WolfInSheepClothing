using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PocketUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;

    public void UpdateItem(ShopItem shopItem)
    {
        itemIcon.sprite = shopItem?.icon;
        itemIcon.color = shopItem != null ? Color.white : Color.clear;
    }
}
