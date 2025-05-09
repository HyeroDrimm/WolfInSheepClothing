using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMenuWindowController : MonoBehaviour
{
    [SerializeField] private MenuFade shopMenu;
    [SerializeField] private MenuFade pauseMenu;
    [SerializeField] private MenuFade settingsMenu;
    [SerializeField] private MenuFade youLostMenu;

    [SerializeField] private float insertShowAt = 0.2f;

    private List<MenuFade> screenObjects = new List<MenuFade>();

    private List<MenuFade> menuStack = new List<MenuFade>();

    private void Awake()
    {
        screenObjects.Add(shopMenu);
        screenObjects.Add(pauseMenu);
        screenObjects.Add(settingsMenu);
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

    public void ShowShopMenu()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(TurnOffAllScreens());
        sequence.Insert(insertShowAt, shopMenu.In());
        menuStack.Append(shopMenu);
        sequence.Play();
    }

    public void ShowPauseMenu(bool state)
    {
        var sequence = DOTween.Sequence();
        if (state)
        {
            sequence.Insert(insertShowAt, pauseMenu.In());
        }
        else
        {
            pauseMenu.Out(out sequence);
        }
        sequence.Play();
    }

    public void ShowSettingsMenu(bool state)
    {
        var sequence = DOTween.Sequence();
        if (state)
        {
            sequence.Append(TurnOffAllScreens());
            sequence.Insert(insertShowAt, settingsMenu.In());
        }
        else
        {
            sequence.Append(TurnOffAllScreens());
            sequence.Insert(insertShowAt, pauseMenu.In());
        }
        sequence.Play();
    }

    public void ShowYouLooseMenu()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(TurnOffAllScreens());
        sequence.Insert(insertShowAt, youLostMenu.In());
        sequence.Play();
    }
}
