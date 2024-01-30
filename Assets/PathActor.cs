using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathActor : MonoBehaviour
{
    [SerializeField] private GameObject startingNode;
    void Start()
    {
        transform.position = startingNode.transform.position;
    }
}
