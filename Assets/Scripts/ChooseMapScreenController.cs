using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseMapScreenController : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    private void Awake()
    {
        level1Button?.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Level1"), "Load Level 1?"));
        level2Button?.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Level2"), "Load Level 2?"));
        level3Button?.onClick.AddListener(() => ConfirmPopup.Instance.Show(() => SceneManager.LoadScene("Level3"), "Load Level 3?"));
    }
}
