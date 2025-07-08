using System;
using System.Collections;
using System.Collections.Generic;
using HyeroUnityEssentials.WindowSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[DefaultExecutionOrder(-1)]
public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject taskUIGameObject;
    [SerializeField] private TMP_Text taskUIText;

    [Header("Clicking on Castles")] 
    [SerializeField] private VideoClip clickingOnCastlesVideo;
    [SerializeField] private String clickingOnCastlesHeaderText;
    [SerializeField] private String clickingOnCastlesTask;
    [SerializeField, Multiline] private String clickingOnCastlesText;
    [SerializeField] private int castlesVisitedRequired;

    [Header("Picking up Pickups")]
    [SerializeField] private VideoClip pickingUpPickupsVideo;
    [SerializeField] private String pickingUpPickupsHeaderText;
    [SerializeField] private String pickingUpPickupsTask;
    [SerializeField, Multiline] private String pickingUpPickupsText;
    [SerializeField] private int pickupsPickedUpRequired;

    [Header("Buying in shop")]
    [SerializeField] private VideoClip buyingInShopVideo;
    [SerializeField] private String buyingInShopHeaderText;
    [SerializeField] private String buyingInShopTask;
    [SerializeField, Multiline] private String buyingInShopText;
    [SerializeField] private int thingsBoughtRequired;

    [Header("Fixing glitches")]
    [SerializeField] private VideoClip fixingGlitchesVideo;
    [SerializeField] private String fixingGlitchesHeaderText;
    [SerializeField] private String fixingGlitchesTask;
    [SerializeField, Multiline] private String fixingGlitchesText;
    [SerializeField] private int fixedGlitchesRequired;

    [Header("Running from Wolf")] 
    [SerializeField] private VideoClip runningFromWolfVideo;
    [SerializeField] private String runningFromWolfHeaderText;
    [SerializeField] private String runningFromWolfTask;
    [SerializeField, Multiline] private String runningFromWolfText;

    private Player player;

    private float timestamp = 0;
    private bool isTimerOn = false;

    private string currentTaskString;
    private int currentTaskMax;

    private int intCounter = 0;
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
        boardManager.SetShopActive(false);
        gameManager.SetEnemyState(false);

        var clickingOnCastlePhase = new TutorialPhase(
            () => intCounter >= castlesVisitedRequired, 
            onStart:()=>
            {
                currentTaskMax = castlesVisitedRequired;
                currentTaskString = clickingOnCastlesTask;

                ResetIntCounter();
                UpdateTaskUI(true, String.Format(currentTaskString, intCounter, currentTaskMax));
                player.onCastleVisited += IncrementIntCounter;
                ModalWindow.ShowOk(clickingOnCastlesHeaderText, clickingOnCastlesText, video: clickingOnCastlesVideo);
            }, 
            onEnd:() =>
            {
                player.onCastleVisited -= IncrementIntCounter;
            });

        var pickingUpPickupsPhase = new TutorialPhase(
            () => intCounter >= pickupsPickedUpRequired,
            onStart: () =>
            {
                boardManager.SetSpawnPowerUp(true);

                currentTaskMax = pickupsPickedUpRequired;
                currentTaskString = pickingUpPickupsTask;

                ResetIntCounter();
                UpdateTaskUI(true, String.Format(currentTaskString, intCounter, currentTaskMax));
                player.onPickup += IncrementIntCounter;
                gameManager.onCollectCoin += IncrementIntCounter;
                ModalWindow.ShowOk(pickingUpPickupsHeaderText, pickingUpPickupsText, video: pickingUpPickupsVideo);
            },
            onEnd: () =>
            {
                gameManager.onCollectCoin -= IncrementIntCounter;
                player.onPickup -= IncrementIntCounter;
            });

        var buyingInShopPhase = new TutorialPhase(
            () => intCounter >= thingsBoughtRequired,
            onStart: () =>
            {
                boardManager.SetShopActive(true);
                currentTaskMax = thingsBoughtRequired;
                currentTaskString = buyingInShopTask;

                ResetIntCounter();
                UpdateTaskUI(true, String.Format(currentTaskString, intCounter, currentTaskMax));
                gameManager.onItemBougth += IncrementIntCounter;
                ModalWindow.ShowOk(buyingInShopHeaderText, buyingInShopText, video: buyingInShopVideo);
            },
            onEnd: () =>
            {
                gameManager.onItemBougth -= IncrementIntCounter;
            });

        var fixingGlitchesPhase = new TutorialPhase(
            () => intCounter >= fixedGlitchesRequired,
            onStart: () =>
            {
                boardManager.SetSpawnGlitches(true);

                currentTaskMax = fixedGlitchesRequired;
                currentTaskString = fixingGlitchesTask;
                ResetIntCounter();
                UpdateTaskUI(true, String.Format(currentTaskString, intCounter, currentTaskMax));
                boardManager.onPlayerFixGlitch += IncrementIntCounter;
                ModalWindow.ShowOk(fixingGlitchesHeaderText, fixingGlitchesText, video: fixingGlitchesVideo);
            },
            onEnd: () =>
            {
                boardManager.onPlayerFixGlitch -= IncrementIntCounter;
            });

        var runningFromEnemyPhase = new TutorialPhase(()=> DidTimePass(2f), onStart:()=>
        {
            ModalWindow.ShowOk(runningFromWolfHeaderText, runningFromWolfText, video: runningFromWolfVideo, onButtonPressed: StartTimer);
        }, onEnd:() =>
        {
        });

        clickingOnCastlePhase.AddNextPhase(pickingUpPickupsPhase);
        pickingUpPickupsPhase.AddNextPhase(buyingInShopPhase);
        buyingInShopPhase.AddNextPhase(fixingGlitchesPhase);
        fixingGlitchesPhase.AddNextPhase(runningFromEnemyPhase);

        fixingGlitchesPhase.Start();
        // clickingOnCastlePhase.Start();
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

    private void IncrementIntCounter()
    {
        intCounter++;
        UpdateTaskUI(true, String.Format(currentTaskString, intCounter, currentTaskMax));
    }

    private void ResetIntCounter()
    {
        intCounter = 0;
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

    public void Setup(Player player)
    {
        this.player = player;
    }
}
