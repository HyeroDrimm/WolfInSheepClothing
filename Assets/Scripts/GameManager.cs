using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Timer currentTimer;
    [SerializeField] private PauseMenuController pauseMenuController;
    [SerializeField] private Timer bestTimer;
    [SerializeField] private YouLostUI youLostUI;
    [SerializeField] private CoinCounterUI coinCounterUi;
    [SerializeField] private CoinCounterUI coinCounterUiShop;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("Shop")]
    [SerializeField] private float shopSlowDown;
    [SerializeField] private float baseGameSpeed;
    [SerializeField] private ShopUI shopUi;

    [Header("Pocket")]
    [SerializeField] private PocketUI pocketUi;
    private ShopItem itemInPocket;

    [Header("Deleting nodes")]
    [SerializeField] private AnimationCurve destructionCurve;
    [SerializeField] private float maxDestructionTime;
    [SerializeField] private int maxDestructorsAtTime;
    [SerializeField] private int newDestructorsAmount;
    [SerializeField] private float startingTimeBetweenDestructors;
    [SerializeField] private float timeBetweenDestructors;
    [SerializeField] private float timeToExplode;


    private bool shouldCountTime = true;
    private float startTimestamp;

    private int coinCounter = 0;
    private float gameSpeed = 1;
    private bool isPause = false;

    private void Awake()
    {
        UpdateTimeScale(0);

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

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            isPause = !isPause;
            SetPause(isPause);
        }
    }

    public void EndGame()
    {
        UpdateTimeScale(0);
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
        coinCounter++;
        coinCounterUi.UpdateCounter(coinCounter);
        coinCounterUiShop.UpdateCounter(coinCounter);
    }

    public void ShowShop(bool state)
    {
        if (state)
        {
            coinCounterUiShop.UpdateCounter(coinCounter);
        }
        shopUi.SetVisible(state);
        UpdateTimeScale(state ? shopSlowDown : baseGameSpeed);
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
        Time.timeScale = state ? 0 : gameSpeed;
        pauseMenuController.SetVisible(state);
    }

    public void UpdateTimeScale(float timeScale)
    {
        gameSpeed = timeScale;
        Time.timeScale = timeScale;
    }

    public void StartGame()
    {
        UpdateTimeScale(1);
    }
}
