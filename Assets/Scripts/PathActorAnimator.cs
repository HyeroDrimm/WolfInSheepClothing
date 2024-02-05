using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathActorAnimator : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimationState(string newState)
    {
        if (newState == currentState) return;

        animator.Play(newState);

        currentState = newState;
    }


}
