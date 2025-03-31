using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FollowGameObject : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    [SerializeField] private float noMoveDistance = 0;
    [SerializeField] private float speed = 0.5f;

    void Update()
    {
        if (followTarget && Vector2.Distance(followTarget.transform.position, transform.position) >= noMoveDistance)
        {
            transform.position = Vector2.Lerp(transform.position, followTarget.transform.position, speed * Time.deltaTime);
        }        
    }
}
