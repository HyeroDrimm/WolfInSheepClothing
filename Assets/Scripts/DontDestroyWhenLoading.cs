using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyWhenLoading : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
