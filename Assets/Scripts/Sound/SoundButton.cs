using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private bool usePositiveSoundOnClick = true;
    [SerializeField] private bool useNegativeSoundOnClick;

    private void Start()
    {
        if (TryGetComponent(out Button button))
        {
            if (usePositiveSoundOnClick)
                button.onClick.AddListener(() =>
                {
                    RuntimeManager.PlayOneShot(SoundManager.Instance.positiveSoundEventID);
                });
            else if (useNegativeSoundOnClick)
                button.onClick.AddListener(() =>
                {
                    RuntimeManager.PlayOneShot(SoundManager.Instance.negativeSoundEventID);
                });
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(SoundManager.Instance.singleSoundEventID);
    }
}
