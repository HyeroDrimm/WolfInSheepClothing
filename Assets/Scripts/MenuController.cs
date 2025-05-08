using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuFade menuScreen;
    [SerializeField] private MenuFade settingsScreen;
    [SerializeField] private MenuFade chooseLevelScreen;
    [SerializeField] private MenuFade creditsScreen;
    [Space]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [Space]
    [SerializeField] private float insertShowAt;

    private List<MenuFade> screenObjects = new List<MenuFade>();


    private void Awake()
    {
        screenObjects.Add(menuScreen);
        screenObjects.Add(settingsScreen);
        screenObjects.Add(chooseLevelScreen);
        screenObjects.Add(creditsScreen);

        playButton.onClick.AddListener(ShowChooseLevelScreen);
        settingsButton.onClick.AddListener(ShowSettingsScreen);
        creditsButton.onClick.AddListener(ShowCreditsScreen);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private Sequence TurnOffAllScreens()
    {
        foreach (var screenObject in screenObjects)
        {
            if (screenObject.Out(out Sequence sequence))
            {
                return sequence;
            }
        }
        return null;
    }

    public void ShowChooseLevelScreen()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(TurnOffAllScreens());
        sequence.Insert(insertShowAt, chooseLevelScreen.In());
        sequence.Play();
    }
    
    public void ShowSettingsScreen()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(TurnOffAllScreens());
        sequence.Insert(insertShowAt, settingsScreen.In());
        sequence.Play();
    }

    public void ShowCreditsScreen()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(TurnOffAllScreens());
        sequence.Insert(insertShowAt, creditsScreen.In());
        sequence.Play();
    }

    public void ShowMenuScreen()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(TurnOffAllScreens());
        sequence.Insert(insertShowAt, menuScreen.In());
        sequence.Play();
    }

    private void OnQuitButtonClicked()
    {
        ConfirmPopup.Instance.Show(Application.Quit,"Quit game?");
    }
}
