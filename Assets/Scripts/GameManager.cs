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
}
