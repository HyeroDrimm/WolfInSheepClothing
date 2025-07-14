using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton?.onClick.AddListener(OnCloseButtonPressed);
    }

    private void OnCloseButtonPressed()
    {
        SetVisible(false);
    }

    public void SetVisible(bool state)
    {
        gameObject.SetActive(state);
    }
}
