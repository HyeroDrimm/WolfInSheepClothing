using System;
using System.Collections;
using System.Collections.Generic;
using HyeroUnityEssentials.WindowSystem;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseMapScreenController : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private GameObject[] levelStars;
    [SerializeField] private TMP_Text[] levelBestTimes;
    [SerializeField] private Curtain curtain;

    IEnumerator AddLevel()
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync("MyLevel", LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone) yield return null;
    }

    private void Awake()
    {
        level1Button?.onClick.AddListener(() => LoadLevel("Level1"));
        level2Button?.onClick.AddListener(() => LoadLevel("Level2"));
        level3Button?.onClick.AddListener(() => LoadLevel("Level3"));
        tutorialButton?.onClick.AddListener(() =>
        {
            GameManager.playTutorial = true;
            curtain.In(() => SceneManager.LoadScene("Level1"));
        });

        float[] bestTimeLevel = {PlayerPrefs.GetFloat("TimeMapLevel1", 0), PlayerPrefs.GetFloat("TimeMapLevel2", 0), PlayerPrefs.GetFloat("TimeMapLevel3", 0) };

        for (int i = 0; i < 9; i++)
        {
            int level = i / 3;
            int star = i % 3;
            float bestTime = bestTimeLevel[level];
            bool starEarned = bestTime > LevelsConsts.timeForLevelStars[level, star];
            levelStars[i].SetActive(starEarned);
            levelBestTimes[level].text = TimeSpan.FromSeconds(bestTime).ToString("mm':'ss");
            levelBestTimes[level].color = Mathf.Approximately(bestTime, 0) ? Color.gray : Color.white;

            if (level == 0 && star == 0)
            {
                level2Button.interactable = starEarned;
            }
            if (level == 1 && star == 0)
            {
                level3Button.interactable = starEarned;
            }
        }
    }

    private void LoadLevel(string levelName)
    {
        bool tutorialPlayed = PlayerPrefs.GetInt("tutorialPlayed", 0) != 0;

        if (tutorialPlayed)
        {
            curtain.In(() => SceneManager.LoadScene(levelName));
        }
        else
        {
            ModalWindow.ShowYesNo("Tutorial", "Do you want to play tutorial first?", 
                onYesAction: 
                () =>
                {
                    GameManager.playTutorial = true;
                    curtain.In(() => SceneManager.LoadScene(levelName));
                }, onNoAction:
                () =>
                {
                    PlayerPrefs.SetInt("tutorialPlayed", 1);
                    curtain.In(() => SceneManager.LoadScene(levelName));
                });
        }
    }
}
