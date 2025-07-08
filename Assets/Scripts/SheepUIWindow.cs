using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using HyeroUnityEssentials.WindowSystem;
using UnityEngine;

public class SheepUIWindow : UIWindow
{
    [SerializeField] private float slideDistance = 10;
    protected override void Awake()
    {
        base.Awake();
        _windowAnimation = new FadeAndShortSlideWindowAnimation(_canvasGroup, _rectTransform, Vector2.right * slideDistance);
    }
}
