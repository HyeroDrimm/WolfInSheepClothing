using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [HideInInspector] public bool isShown = false;

    private void Start()
    {
        SetVisible(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.CollectCoin();
            SetVisible(false);
        }
    }

    public void SetVisible(bool state)
    {
        gameObject.SetActive(state);
        isShown = state;
    }
}
