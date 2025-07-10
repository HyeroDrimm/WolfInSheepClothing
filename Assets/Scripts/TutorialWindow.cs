using System;
using HyeroUnityEssentials;
using HyeroUnityEssentials.WindowSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


[DefaultExecutionOrder(-10)]
public class TutorialWindow : MonoBehaviour
{
    private static TutorialWindow _instance;
    public static TutorialWindow Instance => _instance;

    [SerializeField] private UIWindow uiWindow;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private float timeSlowDown;

    [Header("Header")]
    [SerializeField] private TMP_Text headerText;

    [Header("Vertical Content")]

    [SerializeField] private TMP_Text verticalLayoutText;
    [SerializeField] private RawImage verticalLayoutVideo;

    [Header("Footer")]
    [SerializeField] private Button greenButton;
    [SerializeField] private TMP_Text greenButtonText;


    private Action onGreenButtonPressed;

    private TimeScaleModifier timeScaleModifier;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        timeScaleModifier = new TimeScaleModifier("tutorial window", timeSlowDown, 4);

        _instance = this;
        DontDestroyOnLoad(this);

        greenButton.onClick.AddListener(() => onGreenButtonPressed?.Invoke());
    }

    public static void ShowOk(string headerText, string contentText, VideoClip video = null,
        Action onButtonPressed = null, string buttonText = "Ok", bool zeroTimeScale = true)
    {
        if (Instance == null)
        {
            Debug.LogError("No instance of Modal Window!");
            return;
        }

        Instance.ShowOkImpl(headerText, contentText, video, onButtonPressed, buttonText, zeroTimeScale);
    }

    private void ShowOkImpl(string headerText, string contentText, VideoClip video = null,
        Action onButtonPressed = null, string buttonText = "Ok", bool zeroTimeScale = true)
    {
        this.headerText.text = headerText;

        verticalLayoutVideo.gameObject.SetActive(video != null);
        videoPlayer.clip = video;
        videoPlayer.Play();

        verticalLayoutText.text = contentText;

        greenButtonText.text = buttonText;

        onGreenButtonPressed = WindowManager.Instance.CloseModal;
        if (onButtonPressed != null)
            onGreenButtonPressed += onButtonPressed;

        if (zeroTimeScale)
        {
            TimeController.AddModifier(timeScaleModifier);
            onGreenButtonPressed += () => TimeController.RemoveModifier(timeScaleModifier);
        }

        WindowManager.Instance.ShowModal(uiWindow);
    }
}
