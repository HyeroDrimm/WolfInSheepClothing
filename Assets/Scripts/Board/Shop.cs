using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private GameManager gameManager;
    private bool shouldShowShop = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && shouldShowShop)
        {
            gameManager.ShowShop(true);
        }
    }

    public void SetShopActive(bool state)
    {
        shouldShowShop = state;
    }

    public void Setup(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
