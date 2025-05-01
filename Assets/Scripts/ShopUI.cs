using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button exitButton2;
    [SerializeField] private float shopRefreshDuration;
    [SerializeField] private ShopUIItem[] shopUiItems;
    [SerializeField] private ShopItem[] shopItems;
    [SerializeField] private Popup notEnoughCoinsPopup;
    [SerializeField] private bool skipCheckForCoins;
    [SerializeField] private TMP_Text coinAmount;
    [SerializeField] private MenuFade menuFade;

    //Items


    private void Awake()
    {
        exitButton.onClick.AddListener(OnExitButtonClicked);
        exitButton2.onClick.AddListener(OnExitButtonClicked);

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
            var item = shopItems.Random();
            itemUi.UpdateItem(item);
            itemUi.SetVisible(item.price < gameManager.GetCoins());
        }
    }

    private void OnExitButtonClicked()
    {
        gameManager.ShowShop(false);
    }

    public void SetVisible(bool state)
    {
        if (state)
            menuFade.In().Play();
        else
            menuFade.Out().Play();
    }
}
