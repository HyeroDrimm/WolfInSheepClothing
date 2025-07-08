using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HyeroUnityEssentials.WindowSystem;
using HyeroUnityEssentials;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Timer currentTimer;
    [SerializeField] private UIWindow pauseMenuWindow;
    [SerializeField] private YouLostUI youLostUI;
    [SerializeField] private CoinCounterUI coinCounterUi;
    [SerializeField] private CoinCounterUI coinCounterUiShop;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Tutorial tutorial;

    [Header("Shop")]
    [SerializeField] private float shopSlowDown;
    [SerializeField] private float baseGameSpeed;
    [SerializeField] private UIWindow shopWindow;

    [Header("Pocket")]
    [SerializeField] private PocketUI pocketUi;
    private ShopItem itemInPocket;

    private bool shouldCountTime = true;
    private float startTimestamp;

    private int coinCounter = 0;
    private float gameSpeed = 1;
    private bool isPause = false;

    private bool isEnemyOn = true;

    public Action onCollectCoin;
    public Action onItemBougth;

    // Time scales
    private TimeScaleModifier shopTimeScaleModifier;
    private TimeScaleModifier gameStartTimeScaleModifier;

    // Turn off gamesystems
    public bool isPowerUpOn = true;
    public bool isDestructorsOn = true;

    private void Awake()
    {
        shopTimeScaleModifier = new TimeScaleModifier("shop", shopSlowDown, 2);
        gameStartTimeScaleModifier = new TimeScaleModifier("game start", 0, 5);
        
        TimeController.AddModifier(gameStartTimeScaleModifier);

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

        pocketUi.UpdateItem(itemInPocket);

        if (isEnemyOn)
        {
            SetEnemyState(true);
        }

        tutorial.Setup(player);
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

        if (WindowManager.Instance.IsHistoryEmpty && !pauseMenuWindow.IsVisible && Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            SetPause(true);
        }
    }

    public void EndGame()
    {
        TimeController.Clear(0);

        shouldCountTime = false;
        var time = Time.time - startTimestamp;
        var finalTime = time + coinCounter * 10;
        float oldTime = 0;
        if (PlayerPrefs.HasKey($"TimeMap{SceneManager.GetActiveScene().name}"))
        {
            oldTime = PlayerPrefs.GetFloat($"TimeMap{SceneManager.GetActiveScene().name}");
            if (finalTime > oldTime)
            {
                PlayerPrefs.SetFloat($"TimeMap{SceneManager.GetActiveScene().name}", finalTime);
            }
        }
        else
        {
            PlayerPrefs.SetFloat($"TimeMap{SceneManager.GetActiveScene().name}", finalTime);
        }

        youLostUI.Show(time, finalTime, oldTime, coinCounter);
    }

    public void CollectCoin()
    {
        onCollectCoin?.Invoke();
        coinCounter++;
        coinCounterUi.UpdateCounter(coinCounter);
        coinCounterUiShop.UpdateCounter(coinCounter);
    }

    public void ShowShop(bool state)
    {
        if (state)
        {
            coinCounterUiShop.UpdateCounter(coinCounter);
            WindowManager.Instance.Show(shopWindow);
            TimeController.AddModifier(shopTimeScaleModifier);
        }
        else
        {
            WindowManager.Instance.CloseIfOpen(shopWindow);
            TimeController.RemoveModifier(shopTimeScaleModifier);
        }
    }

    public void UpdatePocket(ShopItem shopItem)
    {
        if (itemInPocket != null)
        {
            onItemBougth?.Invoke();
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
                    enemy.ChangeFreezeAddon(ItemsConsts.POWER_UP_FREEZE_DURATION);
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
            coinCounterUiShop.UpdateCounter(coinCounter);
            return true;
        }

        return false;
    }

    public int GetCoins() => coinCounter;

    public void SetPause(bool state)
    {
        isPause = state;
        Time.timeScale = state ? 0 : gameSpeed;
        if (state)
        {
            WindowManager.Instance.Show(pauseMenuWindow);
        }
    }

    public void UpdateTimeScale(float timeScale)
    {
        gameSpeed = timeScale;
        Time.timeScale = timeScale;
    }

    public void StartGame()
    {
        TimeController.RemoveModifier(gameStartTimeScaleModifier);
    }

    public void SetEnemyState(bool state)
    {
        isEnemyOn = state;
        enemy.gameObject.SetActive(state);
    }
}
