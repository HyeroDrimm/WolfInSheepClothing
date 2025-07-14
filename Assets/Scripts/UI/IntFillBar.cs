using System;
using UnityEngine;
using UnityEngine.UI;

public class IntFillBar : MonoBehaviour
{
    [SerializeField] private Vector2Int minMax;
    [SerializeField] private Image fillImage;
    [SerializeField] private int defaultValue = 0;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    private int d => minMax.y - minMax.x;

    public Action<int, float> OnValueUpdated;

    private int value;
    private float fillAmount;

    private void Awake()
    {
        increaseButton.onClick.AddListener(() => SetValue(value+1));
        decreaseButton.onClick.AddListener(() => SetValue(value-1));

        SetValue(defaultValue);
    }

    public void SetValue(int value)
    {
        if (minMax.x > value || value > minMax.y) return;

        this.value = value;
        this.fillAmount = (float)(value-minMax.x)/(float)(d);
        fillImage.fillAmount = fillAmount;

        OnValueUpdated?.Invoke(this.value, this.fillAmount);
    }

    public void SetFillAmount(float fillAmount)
    {
        if (fillAmount < 0 || 1 < fillAmount) return;
        value = Mathf.RoundToInt(fillAmount * d);
        this.fillAmount = (float)(value - minMax.x) / (float)(minMax.y - minMax.x);
        fillImage.fillAmount = fillAmount;

        OnValueUpdated?.Invoke(this.value, this.fillAmount);
    }

    public int GetValue() => value;
    public float GetFillAmount() => fillAmount;

}
