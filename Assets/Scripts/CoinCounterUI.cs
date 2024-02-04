using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string format;
    [SerializeField] private int defaultValue;

    public void UpdateCounter(int count)
    {
        text.text = string.Format(format, count.ToString());
    }

    private void Start()
    {
        UpdateCounter(defaultValue);
    }
}
