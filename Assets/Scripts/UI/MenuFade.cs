using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MenuFade : MonoBehaviour
{
    [SerializeField] private UISlideIn[] menuElements;
    [SerializeField] private bool state = false;

    private void Awake()
    {
        if (state)
        {
            foreach (var menuElement in menuElements)
            {
                menuElement.InImmediate();
            }
        }
        else
        {
            foreach (var menuElement in menuElements)
            {
                menuElement.OutImmediate();
            }
        }
    }

    public void StartIn()
    {
        var sequence = In();
        sequence.Play();
    }

    public void StartOut()
    {
        var sequence = Out();
        sequence.Play();
    }

    public Sequence In()
    {
        var sequence = DOTween.Sequence();
        if (!state)
        {
            foreach (var menuElement in menuElements)
            {
                var subSequence = menuElement.In();
                sequence.Insert(0, subSequence);
            }
            state = true;
        }

        return sequence;
    }

    public Sequence Out()
    {
        var sequence = DOTween.Sequence();
        if (state)
        {
            foreach (var menuElement in menuElements)
            {
                var subSequence = menuElement.Out();
                sequence.Insert(0, subSequence);
            }
            state = false;
        }
        return sequence;
    }

    public bool Out(out Sequence sequence)
    {
        sequence = DOTween.Sequence();
        if (state)
        {
            foreach (var menuElement in menuElements)
            {
                var subSequence = menuElement.Out();
                sequence.Insert(0, subSequence);
            }
            state = false;
            return true;
        }
        return false;
    }
}
