using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Curtain curtain;

    void Awake()
    {
        StartGame();
    }

    public void StartGame()
    {
        curtain.Out(gameManager.StartGame);
    }
}
