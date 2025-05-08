using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool CanProgress()
    {

        return false;
    }

    private void Progress()
    {

    }

    public enum TutorialStages
    {
        SheepAppears,
        WaitForFirstCastlePress,
        SheepGoesForFirstCastleEnemyAppears,
        WaitForSecondCastlePress,
        WolfGainingOnSheep,
        WaitForThirdCastlePress,

    }
}
