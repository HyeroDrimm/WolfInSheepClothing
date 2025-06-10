using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject taskUIGameObject;
    [SerializeField] private TMP_Text taskUIText;

    [Header("Clicking on Castles")] 
    [SerializeField] private Sprite clickingOnCastlesSprite;
    [SerializeField] private String clickingOnCastlesHeaderText;
    [SerializeField, Multiline] private String clickingOnCastlesText;

    [Header("Running from Wolf")] 
    [SerializeField] private Sprite runningFromWolfSprite;
    [SerializeField] private String runningFromWolfHeaderText;
    [SerializeField, Multiline] private String runningFromWolfText;

    private float timestamp = 0;
    private bool isTimerOn = false;
    private bool DidTimePass(float time)
    {
        if (!isTimerOn) return false;
        
        var state = Time.time - timestamp >= time;
        if (state)
            StopTimer();

        return state;
    }

    private void StopTimer() => isTimerOn = false;
    private void StartTimer()
    {
        timestamp = Time.time;
        isTimerOn = true;
    }


    private void Awake()
    {
        boardManager.SetSpawnGlitches(false);
        boardManager.SetSpawnPowerUp(false);
        gameManager.SetEnemyState(false);

        var clickingOnCastlePhase = new TutorialPhase(()=> DidTimePass(2f), onStart:()=>
        {
            ModalWindow.ShowOk(clickingOnCastlesHeaderText, clickingOnCastlesText, image: clickingOnCastlesSprite, onButtonPressed: StartTimer);
        }, onEnd:() =>
        {
        });

        var runningFromEnemyPhase = new TutorialPhase(()=> DidTimePass(2f), onStart:()=>
        {
            ModalWindow.ShowOk(runningFromWolfHeaderText, runningFromWolfText, image: runningFromWolfSprite, onButtonPressed: StartTimer);
        }, onEnd:() =>
        {
        });

        clickingOnCastlePhase.AddNextPhase(runningFromEnemyPhase);
        clickingOnCastlePhase.Start();
    }

    private void Update()
    {
        TutorialPhase.currentPhase?.Update();
    }

    public void UpdateTaskUI(bool state, string text = "")
    {
        taskUIGameObject.SetActive(state);
        if (state)
        {
            taskUIText.text = text;
        }
    }

    private class TutorialPhase
    {
        public static TutorialPhase currentPhase;

        private Func<bool> checkIfCanProgress;
        private Action onStart;
        private Action onEnd;
        private TutorialPhase nextPhase;
        public TutorialPhase(Func<bool> checkIfCanProgress, Action onStart = null, Action onEnd = null, TutorialPhase nextPhase = null)
        {
            this.checkIfCanProgress = checkIfCanProgress;
            this.onStart = onStart;
            this.nextPhase = nextPhase;
            this.onEnd = onEnd;
        }

        public void Start()
        {
            currentPhase = this;

            onStart?.Invoke();
        }

        public void Update()
        {
            if (checkIfCanProgress.Invoke())
            {
                this.End();
                if (nextPhase != null)
                {
                    nextPhase.Start();
                }
            }
        }

        private void End()
        {
            currentPhase = null;

            onEnd?.Invoke();
        }

        public void AddNextPhase(TutorialPhase nextPhase)
        {
            this.nextPhase = nextPhase;
        }
    }
}
