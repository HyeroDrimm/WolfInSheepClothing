using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathActorAnimator : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    private float scale;

    void Start()
    {
        scale = transform.localScale.x;
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimationState(string newState)
    {
        if (newState != currentState)
        {
            animator.Play(newState);

            currentState = newState;

        }
    }

    public void Flip(bool state)
    {
        transform.localScale = new Vector3(scale *(state ? -1 : 1), transform.localScale.y, transform.localScale.z);
    }

}
