using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyWhenLoading : MonoBehaviour
{
    private static DontDestroyWhenLoading Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
