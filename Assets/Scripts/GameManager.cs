using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Timer currentTimer;
    [SerializeField] private Timer bestTimer;
    [SerializeField] private GameObject youLostUI;
    [SerializeField] private CoinCounterUI coinCounterUi;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("Shop")][SerializeField] private float shopSlowDown;
    [SerializeField] private ShopUI shopUi;

    [Header("Pocket")][SerializeField] private PocketUI pocketUi;
    private ShopItem itemInPocket;


    private bool shouldCountTime = true;
    private float startTimestamp;

    private int coinCounter = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"Too many of {this.GetType()} Deleting this one");
            Destroy(this);
        }
        else
        {
            instance = this;
        }


        startTimestamp = Time.time;
        if (PlayerPrefs.HasKey($"TimeMap{SceneManager.GetActiveScene().name}"))
        {
            bestTimer.UpdateTime(PlayerPrefs.GetFloat($"TimeMap{SceneManager.GetActiveScene().name}"));
        }
        else
        {
            bestTimer.UpdateTime(0f);
        }
    }

    private void Update()
    {
        if (shouldCountTime)
        {
            currentTimer.UpdateTime(Time.time - startTimestamp);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseCurrentPocketItem();
        }
    }

    public void EndGame()
    {
        shouldCountTime = false;
        var finalTime = Time.time - startTimestamp;
        if (PlayerPrefs.HasKey($"TimeMap{SceneManager.GetActiveScene().name}"))
        {
            var oldTime = PlayerPrefs.GetFloat($"TimeMap{SceneManager.GetActiveScene().name}");
            if (finalTime > oldTime)
            {
                PlayerPrefs.SetFloat($"TimeMap{SceneManager.GetActiveScene().name}", finalTime);
            }
        }
        else
        {
            PlayerPrefs.SetFloat($"TimeMap{SceneManager.GetActiveScene().name}", finalTime);
        }

        youLostUI.SetActive(true);
    }

    public void CollectCoin()
    {
        coinCounter++;
        coinCounterUi.UpdateCounter(coinCounter);
    }

    public void ShowShop(bool state)
    {
        shopUi.SetVisible(state);
        Time.timeScale = state ? shopSlowDown : 1;
    }

    public void UpdatePocket(ShopItem shopItem)
    {
        if (itemInPocket != null)
        {
            UsePocketItem(itemInPocket);
        }
        pocketUi.UpdateItem(shopItem);
        itemInPocket = shopItem;
    }

    private void UsePocketItem(ShopItem shopItem)
    {
        if (itemInPocket != null)
        {
            switch (shopItem.type)
            {
                case ItemsConsts.POWER_UP_SPEED_UP:
                    player.PickedUpSpeedChangePowerUp(ItemsConsts.POWER_UP_SPEED_UP_MODIFIER, ItemsConsts.POWER_UP_SPEED_UP_DURATION);
                    break;
                case ItemsConsts.POWER_UP_SPEED_DOWN:
                    enemy.PickedUpSpeedChangePowerUp(ItemsConsts.POWER_UP_SPEED_DOWN_MODIFIER, ItemsConsts.POWER_UP_SPEED_DOWN_DURATION);
                    break;
                case ItemsConsts.POWER_UP_FREEZE:
                    enemy.ChangeWaitAfterMoveAddon(ItemsConsts.POWER_UP_FREEZE_DURATION);
                    break;
                case ItemsConsts.POWER_UP_DOLL:
                    player.UseDoll();
                    break;
                default:
                    Debug.Log("Fucky wacky");
                    break;
            }

            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_UP);
            pocketUi.UpdateItem(null);
        }
    }

    public void UseCurrentPocketItem() => UsePocketItem(itemInPocket);

    public bool PayCoins(int itemPrice)
    {
        if (coinCounter >= itemPrice)
        {
            coinCounter -= itemPrice;
            coinCounterUi.UpdateCounter(coinCounter);
            return true;
        }

        return false;
    }
}
