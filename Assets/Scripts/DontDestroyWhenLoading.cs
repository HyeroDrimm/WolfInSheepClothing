using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyWhenLoading : MonoBehaviour
{
    public static DontDestroyWhenLoading Instance = null;

    void Awake()
    {
        if (Instance == null)
        {
            //if not, set instance to this
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
