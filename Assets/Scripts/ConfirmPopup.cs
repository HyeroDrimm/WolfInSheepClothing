using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : MonoBehaviour
{
    public static ConfirmPopup Instance { get; private set; }

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private MenuFade menuFade;

    private Action currentActionOnConfirm;

    private void Awake()
    {
        if (Instance!=null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);

    }

    public void Show(Action actionOnConfirm, string prompt)
    {
        currentActionOnConfirm = actionOnConfirm;
        menuFade.In().Play();
        promptText.text = prompt;
    }

    public void OnConfirm()
    {
        currentActionOnConfirm?.Invoke();
        menuFade.Out().Play();
    }

    public void OnCancel()
    {
        menuFade.Out().Play();
    }
}
