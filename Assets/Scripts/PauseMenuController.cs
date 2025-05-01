    using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private MenuFade menuFade;

    private void Awake()
    {
        resumeButton?.onClick.AddListener(OnResumeClicked);
        helpButton?.onClick.AddListener(OnHelpClicked);
        mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnResumeClicked()
    {
        gameManager.SetPause(false);
    }

    private void OnHelpClicked()
    {
        throw new System.NotImplementedException();
    }

    private void OnMainMenuClicked()
    {
        ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Menu"), "Exit to Main Menu?");
    }

    public void SetVisible(bool visible)
    {
        if (visible)
            menuFade.In().SetUpdate(true).Play();
        else
            menuFade.Out().SetUpdate(true).Play();
    }
}
