using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouLostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Curtain curtain;
    private const string timePreset = "TIME: {0}";
    private const string cointPreset = "{0}x";
    private const string finalTimePreset = "FINAL TIME: {0}";
    private const string bestTimePreset = "BEST: {0}";

    private void Awake()
    {
        continueButton.onClick.AddListener(()=>curtain.In(()=> SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)));
        quitButton.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => curtain.In(() => SceneManager.LoadScene("Menu")), "Exit to Main Menu?"));
    }

    public void Show(float time, float finalTime, float oldTime, int coinCounter)
    {
        gameObject.SetActive(true);

        timeText.text = string.Format(timePreset, TimeSpan.FromSeconds(time).ToString("mm':'ss"));
        coinText.text = string.Format(cointPreset, coinCounter);
        finalTimeText.text = string.Format(finalTimePreset, TimeSpan.FromSeconds(finalTime).ToString("mm':'ss"));
        bestTimeText.text = string.Format(bestTimePreset, TimeSpan.FromSeconds(oldTime).ToString("mm':'ss"));
     }
}
