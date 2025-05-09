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
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        resumeButton?.onClick.AddListener(OnResumeClicked);
        mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnResumeClicked()
    {
        gameManager.SetPause(false);
    }

    private void OnMainMenuClicked()
    {
        ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Menu"), "Exit to Main Menu?");
    }
}
