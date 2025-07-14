using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string prefix = string.Empty;

    public void UpdateTime(float time)
    {
        text.text = prefix +"\n"+ TimeSpan.FromSeconds(time).ToString("mm':'ss");
    }
}
