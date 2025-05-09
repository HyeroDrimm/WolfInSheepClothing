using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HyeroUnityEssentials.WindowSystem;
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
    [SerializeField] private UIWindow uiWindow;

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
        promptText.text = prompt;
        WindowManager.Instance.ShowModal(uiWindow);
    }

    public void OnConfirm()
    {
        currentActionOnConfirm?.Invoke();
        WindowManager.Instance.CloseModal();
    }

    public void OnCancel()
    {
        WindowManager.Instance.CloseModal();
    }
}
