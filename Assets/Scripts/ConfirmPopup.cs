using System;
using System.Collections;
using System.Collections.Generic;
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

        gameObject.SetActive(false);
    }

    public void Show(Action actionOnConfirm, string prompt)
    {
        currentActionOnConfirm = actionOnConfirm;
        gameObject.SetActive(true);
        promptText.text = prompt;
    }

    public void OnConfirm()
    {
        currentActionOnConfirm?.Invoke();
        gameObject.SetActive(false);
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
