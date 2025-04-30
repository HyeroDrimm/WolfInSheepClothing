using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.GameUI.Explorer;
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
        gameManager.UpdateTimeScale(0);

        curtain.Out(gameManager.StartGame);
    }
}
