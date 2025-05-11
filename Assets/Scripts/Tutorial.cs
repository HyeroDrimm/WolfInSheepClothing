using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject taskUIGameObject;
    [SerializeField] private TMP_Text taskUIText;

    private float timestamp = 0;
    private bool DidTimePass(float time) => Time.time - timestamp >= time;
    private void ResetTimestamp() => timestamp = Time.time;

    private void Awake()
    {
        boardManager.SetSpawnGlitches(false);
        boardManager.SetSpawnPowerUp(false);
        gameManager.SetEnemyState(false);

        var testPhase = new TutorialPhase(()=> DidTimePass(2f), onStart:ResetTimestamp, onEnd:() =>
        {
            Debug.Log("phase 1");
        });
        var testPhase2 = new TutorialPhase(()=> DidTimePass(2f), onStart:ResetTimestamp, onEnd:()=>
        {
            Debug.Log("phase 2");
        });

        testPhase.AddNextPhase(testPhase2);
        testPhase.Start();
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
