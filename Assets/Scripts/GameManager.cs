using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PickUpManager;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Timer currentTimer;
    [SerializeField] private Timer bestTimer;
    [SerializeField] private YouLostUI youLostUI;
    [SerializeField] private CoinCounterUI coinCounterUi;
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
    [SerializeField] private List<Destructor> destructors;
    [SerializeField] private float maxDestructionTime;
    [SerializeField] private int maxDestructorsAtTime;
    [SerializeField] private int newDestructorsAmount;
    [SerializeField] private float startingTimeBetweenDestructors;
    [SerializeField] private float timeBetweenDestructors;
    [SerializeField] private float timeToExplode;


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

        pocketUi.UpdateItem(itemInPocket);

        Time.timeScale = baseGameSpeed;

        InvokeRepeating("SpawnDestructors", startingTimeBetweenDestructors, timeBetweenDestructors);
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

    private void SpawnDestructors()
    {
        var time = Mathf.Clamp01(Time.timeSinceLevelLoad / maxDestructionTime);

        var toSpawnNumber = 2 + Mathf.FloorToInt(destructionCurve.Evaluate(time) * newDestructorsAmount);

        var destructosAvaliable = destructors.Where(x => x.State != Destructor.DestructorState.On && x.State != Destructor.DestructorState.Exploded).ToList();
        var destructosOn = destructors.Where(x => x.State == Destructor.DestructorState.On).ToList();

        for (int i = 0; i < Mathf.Min(maxDestructorsAtTime - destructosOn.Count, toSpawnNumber); i++)
        {
            if (destructosAvaliable.Count != 0)
            {
                var newPickUp = destructosAvaliable.Random();
                newPickUp.StartTimer(timeToExplode);
            }
            else
            {
                break;
            }
        }
    }

    public void EndGame()
    {
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
    }

    public void ShowShop(bool state)
    {
        shopUi.SetVisible(state);
        Time.timeScale = state ? shopSlowDown : baseGameSpeed;
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
            return true;
        }

        return false;
    }
}
