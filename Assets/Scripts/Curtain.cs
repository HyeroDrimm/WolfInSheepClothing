using DG.Tweening.Core.Easing;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

public class Curtain : MonoBehaviour
{
    [SerializeField] private bool inOnStart;
    [SerializeField] private bool openStart;
    [SerializeField] private RectTransform curtainLeft;
    [SerializeField] private RectTransform curtainRight;
    [SerializeField] private float moveDuration;

    private void Awake()
    {
        curtainLeft.anchorMax = new Vector2(inOnStart || openStart ? 0.5f : 0, curtainLeft.anchorMax.y);
        curtainRight.anchorMin = new Vector2(inOnStart || openStart ? 0.5f : 1, curtainLeft.anchorMin.y);

        if (openStart)
        {
            Out();
        }
    }

    public void In(Action actionOnEnd = null)
    {
        DOTween.To(() => curtainLeft.anchorMax.x, value => curtainLeft.anchorMax = new Vector2(value, curtainLeft.anchorMax.y), 0.5f,
            moveDuration).SetUpdate(true);

        var tween = DOTween.To(() => curtainRight.anchorMin.x, value => curtainRight.anchorMin = new Vector2(value, curtainLeft.anchorMin.y), 0.5f,
            moveDuration).SetUpdate(true);

        if (actionOnEnd != null)
        {
            tween.onComplete = actionOnEnd.Invoke;
        }
    }
    public void Out(Action actionOnEnd = null)
    {
        DOTween.To(() => curtainLeft.anchorMax.x, value => curtainLeft.anchorMax = new Vector2(value, curtainLeft.anchorMax.y), 0f,
            moveDuration).SetUpdate(true);

        var tween = DOTween.To(() => curtainRight.anchorMin.x, value => curtainRight.anchorMin = new Vector2(value, curtainLeft.anchorMin.y), 1f,
            moveDuration).SetUpdate(true);

        if (actionOnEnd != null)
        {
            tween.onComplete = actionOnEnd.Invoke;
        }
    }
}
