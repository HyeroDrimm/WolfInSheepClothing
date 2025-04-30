using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelReset : MonoBehaviour
{
    [SerializeField] private Button button;
    void Awake()
    {
        button.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }
}
