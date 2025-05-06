using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private Vector2 minMax;
    [SerializeField] private Image fillImage;
    [SerializeField] private float defaultValue = 0f;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    public Action<float, float> OnValueUpdated;

    private float value;
    private float fillAmount;

    private void Awake()
    {
        increaseButton.onClick.AddListener(() => SetValue(value+1));
        decreaseButton.onClick.AddListener(() => SetValue(value-1));

        SetValue(defaultValue);
    }

    public void SetValue(float value)
    {
        if (minMax.x > value || value > minMax.y) return;

        this.value = value;
        this.fillAmount = (value-minMax.x)/(minMax.y - minMax.x);
        fillImage.fillAmount = fillAmount;

        OnValueUpdated?.Invoke(this.value, this.fillAmount);
    }

    public float GetValue()
    {
        return this.value;
    }
}
