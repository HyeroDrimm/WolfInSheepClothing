using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject chooseLevelScreen;
    [SerializeField] private GameObject creditsScreen;

    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    private List<GameObject> screenObjects = new List<GameObject>();


    private void Awake()
    {
        screenObjects.Add(menuScreen);
        screenObjects.Add(chooseLevelScreen);
        screenObjects.Add(creditsScreen);

        playButton.onClick.AddListener(ShowChooseLevelScreen);
        creditsButton.onClick.AddListener(ShowCreditsScreen);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void TurnOffAllScreens()
    {
        foreach (var screenObject in screenObjects)
        {
            screenObject.SetActive(false);
        }
    }

    public void ShowChooseLevelScreen()
    {
        TurnOffAllScreens();
        chooseLevelScreen.SetActive(true);
    }

    public void ShowCreditsScreen()
    {
        TurnOffAllScreens();
        creditsScreen.SetActive(true);
    }

    public void ShowMenuScreen()
    {
        TurnOffAllScreens();
        menuScreen.SetActive(true);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
