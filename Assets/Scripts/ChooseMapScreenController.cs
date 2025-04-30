using Codice.Client.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseMapScreenController : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private GameObject[] levelStars;
    [SerializeField] private TMP_Text[] levelBestTimes;

    private void Awake()
    {
        level1Button?.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Level1"), "Load Level 1?"));
        level2Button?.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Level2"), "Load Level 2?"));
        level3Button?.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Level3"), "Load Level 3?"));

        float[] bestTimeLevel = {PlayerPrefs.GetFloat("TimeMapLevel1", 0), PlayerPrefs.GetFloat("TimeMapLevel2", 0), PlayerPrefs.GetFloat("TimeMapLevel3", 0) };

        for (int i = 0; i < 9; i++)
        {
            int level = i / 3;
            int star = i % 3;
            float bestTime = bestTimeLevel[level];
            levelStars[i].SetActive(bestTime > LevelsConsts.timeForLevelStars[level, star]);
            levelBestTimes[level].text = TimeSpan.FromSeconds(bestTime).ToString("mm':'ss");
            levelBestTimes[level].color = Mathf.Approximately(bestTime, 0) ? Color.gray : Color.white;
        }
    }
}
