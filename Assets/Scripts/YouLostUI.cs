using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YouLostUI : MonoBehaviour
{
    [SerializeField] private string preset;
    [SerializeField] private string oldTimeBeatenPreset;
    [SerializeField] private string oldTimeNotBeatenPreset;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text oldTimeText;

    public void Show(float time, float finalTime, float oldTime, int coinCounter)
    {
        gameObject.SetActive(true);
        finalTimeText.text = string.Format(preset, Mathf.FloorToInt(finalTime), Mathf.FloorToInt(time), coinCounter);
        oldTimeText.text = oldTime < finalTime ? string.Format(oldTimeBeatenPreset, Mathf.FloorToInt(oldTime)) : string.Format(oldTimeNotBeatenPreset, Mathf.FloorToInt(oldTime));
    }
}
