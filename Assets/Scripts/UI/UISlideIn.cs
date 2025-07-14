using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class UISlideIn : MonoBehaviour
{
    [SerializeField] private Vector2 slideFrom;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float timeOffset;

    private CanvasGroup canvas;
    private RectTransform rectTransform;
    private Vector3 basePosition;

    private void Awake()
    {
        TryGetComponent(out canvas);
        TryGetComponent(out rectTransform);
        if (rectTransform)
        {
            basePosition = rectTransform.position;
        }
    }

    public Sequence In()
    {
        var sequence = DOTween.Sequence();
        if (canvas)
        {
            canvas.alpha = 0;
            var tween = DOTween.To(() => canvas.alpha, f => canvas.alpha = f, 1, duration);
            tween.onComplete = () => canvas.blocksRaycasts = true;
            tween.SetUpdate(true);
            sequence.Insert(timeOffset, tween);
        }

        if (rectTransform)
        {
            rectTransform.position += new Vector3(slideFrom.x, slideFrom.y);
            var tween = DOTween.To(() => rectTransform.position, f => rectTransform.position = f, basePosition, duration);
            tween.SetUpdate(true);
            sequence.Insert(timeOffset, tween);
        }

        return sequence;
    }

    public Sequence Out()
    {
        var sequence = DOTween.Sequence();

        if (canvas)
        {
            canvas.blocksRaycasts = false;
            canvas.alpha = 1;
            var tween = DOTween.To(() => canvas.alpha, f => canvas.alpha = f, 0, duration);
            tween.SetUpdate(true);
            sequence.Insert(timeOffset, tween);
        }

        if (rectTransform)
        {
            var targetPosition = basePosition + new Vector3(slideFrom.x, slideFrom.y);
            var tween = DOTween.To(() => rectTransform.position, f => rectTransform.position = f, targetPosition, duration);
            tween.SetUpdate(true);
            sequence.Insert(timeOffset, tween);
        }

        return sequence;
    }

    public void OutImmediate()
    {
        if (!canvas) return;
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
    }

    public void InImmediate()
    {
        if (!canvas) return;
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
    }
}
