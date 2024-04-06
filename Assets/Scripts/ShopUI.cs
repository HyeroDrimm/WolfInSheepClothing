using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button exitButton;
    [SerializeField] private float shopRefreshDuration;
    [SerializeField] private ShopUIItem[] shopUiItems;
    [SerializeField] private ShopItem[] shopItems;
    [SerializeField] private Popup notEnoughCoinsPopup;
    [SerializeField] private bool skipCheckForCoins;

    //Items


    private void Awake()
    {
        exitButton.onClick.AddListener(OnExitButtonClicked);

        foreach (var itemUi in shopUiItems)
        {
            itemUi.OnItemPicked += OnItemPickedUp;
        }

        InvokeRepeating("RefreshShop", 0, shopRefreshDuration);

        SetVisible(false);
    }

    private bool OnItemPickedUp(ShopItem item)
    {
        if (gameManager.PayCoins(item.price) || skipCheckForCoins)
        {
            gameManager.UpdatePocket(item);
            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.KACHING);
            OnExitButtonClicked();
            return true;
        }
        notEnoughCoinsPopup.SetVisible(true);
        return false;
    }

    private void RefreshShop()
    {
        foreach (var itemUi in shopUiItems)
        {
            itemUi.UpdateItem(shopItems.Random());
            itemUi.SetVisible(true);
        }
    }

    private void OnExitButtonClicked()
    {
        gameManager.ShowShop(false);
    }

    public void SetVisible(bool state)
    {
        gameObject.SetActive(state);
    }
}
