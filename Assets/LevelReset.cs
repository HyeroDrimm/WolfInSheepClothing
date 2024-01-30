using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelReset : MonoBehaviour
{
    [SerializeField] private Button buttonn;
    void Awake()
    {
        if (TryGetComponent(out Button button))
        {
            //buttonn. += () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}
